using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CommandLine;
using FindDupes.Hash;
using FindDupes.Interfaces;
using FindDupes.Structs;

namespace FindDupes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //General overview:
            // The algorithm here is to read files one at the time, and then pass each file to one of the hashing threads.
            // We only have a single thread reading from the harddisk to optimize throughput and keep the drive busy 100% of the time.
            // It reads in 32MB chunks, which is the maximum buffer capacity of modern drives.

            //TODO:
            // Filename similarity
            // Partial mode

            ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);

            if (!(parserResult is Parsed<Options> parsedOptions))
                return;

            Options o = parsedOptions.Value;

            int numProcesses = o.ThreadCount;

            if (numProcesses == 0)
                numProcesses = Environment.ProcessorCount;

            if (o.DryRun)
                Console.WriteLine("Dry-run enabled. Files won't actually be deleted.");

            if (o.ShowProgress)
                Console.WriteLine("Looking for files...");

            Stopwatch sw = null;
            TimeSpan filesElapsed = TimeSpan.Zero;

            if (o.ShowTiming)
                sw = Stopwatch.StartNew();

            IList<FileDetails> files = GetFiles(o.Directory, o.RecurseSubdirectories, new FileFilter(o.MinSize, o.MaxSize, o.SkipHidden)).ToList();

            if (sw != null)
                filesElapsed = sw.Elapsed;

            QueueStatus status = new QueueStatus(numProcesses, files.Count, files.Sum(x => x.Size));

            Dictionary<FileDetails, IList<FileDetails>> duplicates = new Dictionary<FileDetails, IList<FileDetails>>(new FileDetailsComparer(!o.DisableSize, o.EnableTimestamp, !o.DisableHash));

            ActionBlock<FileData> hashBlock = new ActionBlock<FileData>(x =>
            {
                FileDetails details = x.FileDetails;

                IFileHash hash;

                if (o.UseFastHash)
                    hash = new xxHash();
                else
                    hash = new SHA1Hash();

                details.Hash = hash.GetHash(x.Data);

                lock (duplicates)
                {
                    if (duplicates.ContainsKey(details))
                        duplicates[details].Add(details);
                    else
                        duplicates.Add(details, new List<FileDetails> { details });
                }

                lock (status)
                {
                    status.IncrementQueue(Thread.CurrentThread.ManagedThreadId);
                    status.IncrementBytes(details.Size);

                    if (o.ShowProgress)
                        status.PrintStatus(status);
                }

            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = numProcesses, BoundedCapacity = o.MaxInMemory });

            sw?.Restart();

            if (o.ShowProgress)
                status.PrintStatus(status);

            foreach (FileDetails file in files)
            {
                byte[] data = null;

                try
                {
                    using (FileStream fs = new FileStream(file.Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        data = new byte[fs.Length];
                        fs.Read(data, 0, data.Length);
                    }
                }
                catch (Exception)
                {
                    //ignore
                }

                if (data == null)
                {
                    lock (status)
                    {
                        status.IncrementQueue(Thread.CurrentThread.ManagedThreadId);
                        status.IncrementBytes(file.Size);
                    }
                }
                else
                {
                    while (!hashBlock.Post(new FileData(data, file)))
                    {
                        await Task.Delay(10);
                    }
                }
            }

            hashBlock.Complete();
            await hashBlock.Completion;

            if (sw != null)
                Console.WriteLine($"It took {filesElapsed} to find files. It took {sw.Elapsed} to check for duplicates.");

            List<KeyValuePair<FileDetails, IList<FileDetails>>> dupes = duplicates.Where(x => x.Value.Count >= 2).ToList();

            if (o.NoAsk)
            {
                foreach ((FileDetails _, IList<FileDetails> value) in dupes)
                {
                    foreach (FileDetails details in value.Skip(1))
                    {
                        DeleteFile(details.Filename, o.DryRun);
                    }
                }
            }
            else
                HandleDupes(dupes, o.DryRun);
        }

        private static void HandleDupes(ICollection<KeyValuePair<FileDetails, IList<FileDetails>>> dupes, bool dryRun)
        {
            Console.WriteLine($"Found {dupes.Count} duplicate{(dupes.Count != 1 ? "s" : string.Empty)}");

            if (dupes.Count == 0)
            {
                Console.WriteLine("Nothing to do. Exiting.");
                return;
            }

            char pressedKey;

            do
            {
                Console.WriteLine();
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1. Review");
                Console.WriteLine("2. Auto-delete");
                Console.WriteLine("3. Exit");

                pressedKey = Console.ReadKey(true).KeyChar;
            } while (pressedKey != '1' && pressedKey != '2' && pressedKey != '3');

            switch (pressedKey)
            {
                case '1':
                    foreach ((FileDetails _, IList<FileDetails> value) in dupes)
                    {
                        int delKey;
                        do
                        {
                            Console.WriteLine();
                            Console.WriteLine("Choose which file to keep and press Enter:");

                            for (int i = 0; i < value.Count; i++)
                            {
                                FileDetails details = value[i];
                                Console.WriteLine($"{(i + 1)}. {details.Filename}");
                            }

                            Console.WriteLine($"{value.Count + 1}. Skip");

                            int.TryParse(Console.ReadLine(), out delKey);

                        } while (delKey <= 0 || delKey > value.Count + 1);

                        if (delKey == value.Count + 1)
                            continue;

                        for (int i = 0; i < value.Count; i++)
                        {
                            if (i + 1 == delKey)
                                continue;

                            DeleteFile(value[i].Filename, dryRun);
                        }
                    }
                    break;
                case '2':
                    Console.WriteLine();

                    foreach ((FileDetails _, IList<FileDetails> value) in dupes)
                    {
                        foreach (FileDetails details in value.Skip(1))
                        {
                            DeleteFile(details.Filename, dryRun);
                        }
                    }
                    break;
                case '3':
                    Console.WriteLine("No action taken. Exiting.");
                    break;
            }
        }

        private static void DeleteFile(string file, bool dryRun)
        {
            Console.WriteLine($"Deleting {file}");

            if (!dryRun)
                File.Delete(file);
        }

        private static IEnumerable<FileDetails> GetFiles(string directory, bool recursive, FileFilter filter)
        {
            FileSystemEnumerable<FileDetails> filesystemEnum = new FileSystemEnumerable<FileDetails>(directory, (ref FileSystemEntry entry) => new FileDetails(entry.ToSpecifiedFullPath(), entry.Length, entry.CreationTimeUtc), new EnumerationOptions { AttributesToSkip = FileAttributes.ReparsePoint, IgnoreInaccessible = true, RecurseSubdirectories = recursive });
            filesystemEnum.ShouldIncludePredicate = (ref FileSystemEntry entry) => filter.ShouldInclude(entry.IsDirectory, entry.Length, entry.IsHidden);

            foreach (FileDetails file in filesystemEnum)
            {
                yield return file;
            }
        }
    }
}
