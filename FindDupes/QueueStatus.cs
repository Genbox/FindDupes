using System;
using System.Collections.Generic;
using System.Linq;

namespace FindDupes
{
    public class QueueStatus
    {
        private readonly Dictionary<int, int> _queues;
        private readonly int _currentTop;

        public QueueStatus(int numQueues, int filesTotal, long bytesTotal)
        {
            FilesTotal = filesTotal;
            BytesTotal = bytesTotal;

            _currentTop = Console.CursorTop;
            _queues = new Dictionary<int, int>(numQueues);
        }

        public int FilesTotal { get; }
        public int FilesProgress { get; private set; }

        public long BytesTotal { get; }
        public long BytesProgress { get; private set; }

        public void IncrementBytes(long bytes)
        {
            BytesProgress += bytes;
        }

        public void IncrementQueue(int index)
        {
            FilesProgress++;

            if (_queues.ContainsKey(index))
                _queues[index]++;
            else
                _queues.Add(index, 1);
        }

        public float GetTotalProgress()
        {
            float pct = (float)FilesProgress / FilesTotal;
            return float.IsNaN(pct) ? 0f : pct;
        }

        public int[] GetQueueStatus()
        {
            return _queues.Select(x => x.Value).ToArray();
        }

        public void PrintStatus(QueueStatus status)
        {
            Console.SetCursorPosition(0, _currentTop);

            //int[] ints = status.GetQueueStatus();

            //for (int i = 0; i < ints.Length; i++)
            //{
            //    Console.Write($"Thread {i} : {ints[i]}");
            //    Console.Write(string.Concat(Enumerable.Repeat(" ", Console.WindowWidth - Console.CursorLeft)));
            //}

            Console.Write($"Total progress: {status.FilesProgress}/{status.FilesTotal} - {BytesProgress / 1024 / 1024:N0}/{BytesTotal / 1024 / 1024:N0} MiB - {status.GetTotalProgress() * 100.0f:0.00} %");
            Console.Write(string.Concat(Enumerable.Repeat(" ", Console.WindowWidth - Console.CursorLeft)));
        }
    }
}