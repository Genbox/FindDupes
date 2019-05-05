using System;

namespace FindDupes.Structs
{
    public struct FileDetails
    {
        public string Filename { get; }
        public long Size { get; }
        public DateTimeOffset Timestamp { get; }
        public ulong Hash { get; set; }

        public FileDetails(string filename, long size, DateTimeOffset timestamp, ulong hash = 0)
        {
            Filename = filename;
            Size = size;
            Timestamp = timestamp;
            Hash = hash;
        }

        public override string ToString()
        {
            return $"{Size}-{Timestamp}-{Hash}";
        }
    }
}