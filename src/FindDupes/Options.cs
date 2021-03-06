﻿using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace FindDupes
{
    public class Options
    {
        [Option("enable-timestamp", Required = false, HelpText = "Use timestamps as a criteria for comparing duplicates.")]
        public bool EnableTimestamp { get; set; }

        [Option("disable-hash", Required = false, HelpText = "Do not use hash as a criteria for comparing duplicates.")]
        public bool DisableHash { get; set; }

        [Option("min-size", Required = false, HelpText = "The minimum size in bytes as a filter for duplicates.")]
        public long MinSize { get; set; }

        [Option("max-size", Default = long.MaxValue, Required = false, HelpText = "The maximum size in bytes as a filter for duplicates.")]
        public long MaxSize { get; set; }

        [Option('f', "fasthash", Required = false, HelpText = "Use a faster hash function.")]
        public bool UseFastHash { get; set; }

        [Option("skip-hidden", Required = false, HelpText = "Skip hidden files.")]
        public bool SkipHidden { get; set; }

        [Option("no-ask", Required = false, HelpText = "Don't ask to delete files, just delete them.")]
        public bool NoAsk { get; set; }

        [Option('p', "progress", Required = false, HelpText = "Show progress bar.")]
        public bool ShowProgress { get; set; }

        [Option("timing", Required = false, HelpText = "Show timing information.")]
        public bool ShowTiming { get; set; }

        [Option('r', "recursive", Required = false, HelpText = "Go into sub-directories recursively.")]
        public bool RecurseSubdirectories { get; set; }

        [Option("dry", Required = false, HelpText = "Do a dry-run. Don't actually delete files.")]
        public bool DryRun { get; set; }

        [Value(0, MetaName = "Directory", Required = true, HelpText = "The directory to remove duplicates from.")]
        public string Directory { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Find all duplicates in a folder", new Options { Directory = @"C:\MyDirectory" });
                yield return new Example("Skip empty files", new Options { Directory = @"C:\MyDirectory", MinSize = 1 });
                yield return new Example("Delete all duplicate files automatically", new Options { Directory = @"C:\MyDirectory", RecurseSubdirectories = true, NoAsk = true });
            }
        }
    }
}