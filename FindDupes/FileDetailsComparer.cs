using System.Collections.Generic;
using FindDupes.Structs;

namespace FindDupes
{
    public class FileDetailsComparer : IEqualityComparer<FileDetails>
    {
        public bool UseSize { get; }
        public bool UseTimestamp { get; }
        public bool UseHash { get; }

        public FileDetailsComparer(bool useSize, bool useTimestamp, bool useHash)
        {
            UseSize = useSize;
            UseTimestamp = useTimestamp;
            UseHash = useHash;
        }

        public bool Equals(FileDetails x, FileDetails y)
        {
            if (UseSize && x.Size != y.Size)
                return false;

            if (UseTimestamp && !x.Timestamp.Equals(y.Timestamp))
                return false;

            if (UseHash && x.Hash != y.Hash)
                return false;

            return true;
        }

        public int GetHashCode(FileDetails obj)
        {
            unchecked
            {
                int hashCode = 7;

                if (UseSize)
                    hashCode = (hashCode * 397) ^ obj.Size.GetHashCode();

                if (UseTimestamp)
                    hashCode = (hashCode * 397) ^ obj.Timestamp.GetHashCode();

                if (UseHash)
                    hashCode = (hashCode * 397) ^ obj.Hash.GetHashCode();

                return hashCode;
            }
        }
    }
}