namespace FindDupes.Structs
{
    internal readonly struct FileData
    {
        public byte[] Data { get; }
        public FileDetails FileDetails { get; }

        public FileData(byte[] data, FileDetails fileDetails)
        {
            Data = data;
            FileDetails = fileDetails;
        }
    }
}