using System;

namespace FindDupes
{
    public class FileDetails
    {
        public FileDetails(string filename, long size, DateTimeOffset timestamp, ulong hash = 0)
        {
            Filename = filename;
            Size = size;
            Timestamp = timestamp;
            Hash = hash;
        }

        public string Filename { get; }
        public long Size { get; }
        public DateTimeOffset Timestamp { get; }
        public ulong Hash { get; set; }

        public override string ToString()
        {
            return $"{Filename} || {Size} || {Timestamp} || {Hash}";
        }
    }
}