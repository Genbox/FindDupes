using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using CommandLine;
using FindDupes.Abstracts;
using FindDupes.Hash;

namespace FindDupes
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //TODO:
            // Filename similarity
            // Partial mode

            ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);

            if (!(parserResult is Parsed<Options> parsedOptions))
                return;

            Options o = parsedOptions.Value;

            if (o.DryRun)
                Console.WriteLine("Dry-run enabled. Files won't actually be deleted.");

            if (o.ShowProgress)
                Console.WriteLine("Looking for files...");

            Stopwatch sw = null;
            TimeSpan filesElapsed = TimeSpan.Zero;

            if (o.ShowTiming)
                sw = Stopwatch.StartNew();

            List<FileDetails> files = GetFiles(o.Directory, o.RecurseSubdirectories, o.EnableTimestamp, new FileFilter(o.MinSize, o.MaxSize, o.SkipHidden)).ToList();

            if (sw != null)
                filesElapsed = sw.Elapsed;

            //Group files by size. This gives us groups with each more than 2 candidates.
            List<List<FileDetails>> sizeGroups = GroupBySize(files).ToList();

            QueueStatus status = new QueueStatus(sizeGroups.Sum(x => x.Count), sizeGroups.Sum(x => x.Sum(y => y.Size)));

            if (o.ShowProgress)
                status.PrintStatus(status);

            sw?.Restart();

            if (!o.DisableHash)
                HashFiles(sizeGroups, status, o.ShowProgress, o.UseFastHash);
            else
            {
                status.BytesProgress = status.BytesTotal;
                status.FilesProgress = status.FilesTotal;
            }

            if (o.ShowProgress)
                status.PrintStatus(status);

            if (sw != null)
                Console.WriteLine($"It took {filesElapsed} to find files. It took {sw.Elapsed} to hash files.");

            List<List<FileDetails>> withCriteria = GroupByCriteria(sizeGroups).ToList();

            if (o.NoAsk)
                DeleteAllButFirst(withCriteria, o.DryRun);
            else
                HandleDupes(withCriteria, o.DryRun);
        }

        private static IEnumerable<List<FileDetails>> GroupByCriteria(List<List<FileDetails>> sizeGroups)
        {
            FileDetailsComparer comparer = new FileDetailsComparer();
            return sizeGroups.SelectMany(a => a.GroupBy(b => b, (details, list) => list.ToList(), comparer)).Where(x => x.Count >= 2);
        }

        private static IEnumerable<List<FileDetails>> GroupBySize(ICollection<FileDetails> files)
        {
            return files.GroupBy(x => x.Size, (details, list) => list.ToList()).Where(x => x.Count >= 2);
        }

        private static void HashFiles(List<List<FileDetails>> sizeGroups, QueueStatus status, bool showProgress, bool useFastHash)
        {
            IFileHash hash;

            if (useFastHash)
                hash = new xxHash();
            else
                hash = new SHA1Hash();

            //Hash all dupes in each group
            foreach (List<FileDetails> sizeGroup in sizeGroups)
            {
                foreach (FileDetails details in sizeGroup)
                {
                    if (TryReadFile(details, out byte[] data))
                        details.Hash = hash.GetHash(data);

                    status.Increment(details.Size);

                    if (showProgress)
                        status.PrintStatus(status);
                }
            }
        }

        private static void HandleDupes(List<List<FileDetails>> groups, bool dryRun)
        {
            Console.WriteLine($"Found {groups.Count} files with duplicate{(groups.Count != 1 ? "s" : string.Empty)}");

            if (groups.Count == 0)
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
                    HandleSelectItem(groups, dryRun);
                    break;
                case '2':
                    Console.WriteLine();
                    DeleteAllButFirst(groups, dryRun);
                    break;
                case '3':
                    Console.WriteLine("No action taken. Exiting.");
                    break;
            }
        }

        private static void HandleSelectItem(List<List<FileDetails>> groups, bool dryRun)
        {
            foreach (List<FileDetails> group in groups)
            {
                string input;
                int key;
                do
                {
                    Console.WriteLine();
                    Console.WriteLine("Choose which file to keep and press Enter:");

                    for (int i = 0; i < group.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {group[i].Filename}");
                    }

                    Console.WriteLine("S. Skip");

                    input = Console.ReadLine() ?? string.Empty;
                    int.TryParse(input, out key);

                } while (!input.Equals("s", StringComparison.OrdinalIgnoreCase) && (key <= 0 || key > group.Count));

                //Was skipped pressed?
                if (input.Equals("s", StringComparison.OrdinalIgnoreCase))
                    continue;

                //Find the file that was selected and delete all others
                for (int i = 0; i < group.Count; i++)
                {
                    if (i + 1 == key)
                        continue;

                    DeleteFile(group[i].Filename, dryRun);
                }
            }
        }

        private static void DeleteAllButFirst(List<List<FileDetails>> groups, bool dryRun)
        {
            foreach (List<FileDetails> group in groups)
            {
                foreach (FileDetails details in group.Skip(1))
                {
                    DeleteFile(details.Filename, dryRun);
                }
            }
        }

        private static bool TryReadFile(FileDetails details, out byte[] data)
        {
            try
            {
                using (FileStream fs = new FileStream(details.Filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                }

                return true;
            }
            catch (Exception)
            {
                //ignore
            }

            data = null;
            return false;
        }

        private static void DeleteFile(string file, bool dryRun)
        {
            Console.WriteLine($"Deleting {file}");

            if (!dryRun)
                File.Delete(file);
        }

        private static IEnumerable<FileDetails> GetFiles(string directory, bool recursive, bool enableTimestamp, FileFilter filter)
        {
            //FileSystemEntry is lazy loaded, so we touch it as little as possible if we can
            EnumerationOptions options = new EnumerationOptions { AttributesToSkip = FileAttributes.ReparsePoint, IgnoreInaccessible = true, RecurseSubdirectories = recursive };
            FileSystemEnumerable<FileDetails> filesystemEnum = new FileSystemEnumerable<FileDetails>(directory, (ref FileSystemEntry entry) => new FileDetails(entry.ToSpecifiedFullPath(), entry.Length, enableTimestamp ? entry.LastWriteTimeUtc : DateTimeOffset.MinValue), options);
            filesystemEnum.ShouldIncludePredicate = filter.ShouldInclude;
            return filesystemEnum;
        }
    }
}