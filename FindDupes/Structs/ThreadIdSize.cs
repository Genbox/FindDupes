namespace FindDupes.Structs
{
    public readonly struct ThreadIdSize
    {
        public int ThreadId { get; }
        public long Size { get; }

        public ThreadIdSize(int threadId, long size)
        {
            ThreadId = threadId;
            Size = size;
        }
    }
}