using System;
using System.Linq;

namespace FindDupes
{
    public class QueueStatus
    {
        private readonly int _currentTop;

        public QueueStatus(int filesTotal, long bytesTotal)
        {
            FilesTotal = filesTotal;
            BytesTotal = bytesTotal;

            _currentTop = Console.CursorTop;
        }

        public int FilesTotal { get; }
        public int FilesProgress { get; set; }

        public long BytesTotal { get; }
        public long BytesProgress { get; set; }

        public void Increment(long bytes)
        {
            BytesProgress += bytes;
            FilesProgress++;
        }

        public float GetTotalProgress()
        {
            float pct = (float)FilesProgress / FilesTotal;
            return float.IsNaN(pct) ? 0f : pct;
        }

        public void PrintStatus(QueueStatus status)
        {
            Console.SetCursorPosition(0, _currentTop);
            Console.Write($"Total progress: {status.FilesProgress}/{status.FilesTotal} - {BytesProgress / 1024 / 1024:N0}/{BytesTotal / 1024 / 1024:N0} MiB - {status.GetTotalProgress() * 100.0f:0.00} %");
            Console.Write(string.Concat(Enumerable.Repeat(" ", Console.WindowWidth - Console.CursorLeft)));
        }
    }
}