using CommandLine;

namespace FindDupes
{
    public class Options
    {
        [Option("disable-size", Required = false, HelpText = "Do not use size as a criteria for comparing duplicates.")]
        public bool DisableSize { get; set; }

        [Option("enable-timestamp", Required = false, HelpText = "Use timestamp as a criteria for comparing duplicates.")]
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

        [Option('t', "threads", Required = false, HelpText = "The number of threads to use. By default it uses all available cores.")]
        public int ThreadCount { get; set; }

        [Option('p', "progress", Required = false, HelpText = "Show progress bar.")]
        public bool ShowProgress { get; set; }

        [Option("timing", Required = false, HelpText = "Show timing information.")]
        public bool ShowTiming { get; set; }

        [Option('r', "recursive", Required = false, HelpText = "Go into sub-directories recursively.")]
        public bool RecurseSubdirectories { get; set; }

        [Option("dry", Required = false, HelpText = "Do a dry-run. Don't actually delete files.")]
        public bool DryRun { get; set; }

        [Option("max-files", Default = 10, Required = false, HelpText = "Maximum number of files to keep in memory at once.")]
        public int MaxInMemory { get; set; }

        [Value(0, MetaName = "Directory", Required = true, HelpText = "The directory to remove duplicates from.")]
        public string Directory { get; set; }
    }
}