using System.Collections.Generic;

namespace FindDupes
{
    public class FileDetailsComparer : IEqualityComparer<FileDetails>
    {
        public bool Equals(FileDetails x, FileDetails y)
        {
            if (!x.Timestamp.Equals(y.Timestamp))
                return false;

            if (x.Hash != y.Hash)
                return false;

            return true;
        }

        public int GetHashCode(FileDetails obj)
        {
            unchecked
            {
                int hashCode = 7;
                hashCode = (hashCode * 397) ^ obj.Timestamp.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Hash.GetHashCode();
                return hashCode;
            }
        }
    }
}