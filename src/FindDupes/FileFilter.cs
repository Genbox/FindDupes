using System.IO.Enumeration;

namespace FindDupes
{
    public class FileFilter
    {
        private readonly long _maxSize;
        private readonly long _minSize;
        private readonly bool _skipHidden;

        public FileFilter(long minSize, long maxSize, bool skipHidden)
        {
            _minSize = minSize;
            _maxSize = maxSize;
            _skipHidden = skipHidden;
        }

        public bool ShouldInclude(ref FileSystemEntry entry)
        {
            if (entry.IsDirectory)
                return false;

            long length = entry.Length;

            if (length < _minSize)
                return false;

            if (length >= _maxSize)
                return false;

            if (_skipHidden && entry.IsHidden)
                return false;

            return true;
        }
    }
}