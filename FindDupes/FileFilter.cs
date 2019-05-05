namespace FindDupes
{
    public sealed class FileFilter
    {
        private readonly long _minSize;
        private readonly long _maxSize;
        private readonly bool _skipHidden;

        public FileFilter(long minSize, long maxSize, bool skipHidden)
        {
            _minSize = minSize;
            _maxSize = maxSize;
            _skipHidden = skipHidden;
        }

        public bool ShouldInclude(bool isDirectory, long length, bool isHidden)
        {
            if (isDirectory)
                return false;

            if (length < _minSize)
                return false;

            if (length > _maxSize)
                return false;

            if (_skipHidden && isHidden)
                return false;

            return true;
        }
    }
}