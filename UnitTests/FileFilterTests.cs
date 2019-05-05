using FindDupes;
using Xunit;

namespace UnitTests
{
    public class FileFilterTests
    {
        [Theory]
        [InlineData(false, 1, false, 0, long.MaxValue, false)]
        [InlineData(false, 1, false, 1, 1, false)]
        public void ShouldIncludeTrue(bool isDirectory, long length, bool isHidden, long minSize, long maxSize, bool skipHidden)
        {
            FileFilter f = new FileFilter(minSize, maxSize, skipHidden);
            Assert.True(f.ShouldInclude(isDirectory, length, isHidden));
        }

        [Theory]
        [InlineData(true, 1, false, 0, long.MaxValue, false)] //It is a directory
        [InlineData(false, 0, false, 1, 3, false)] //Size is too small
        [InlineData(false, 4, false, 1, 3, false)] //Size is too large
        [InlineData(false, 2, true, 1, 3, true)] //File is hidden
        public void ShouldIncludeFalse(bool isDirectory, long length, bool isHidden, long minSize, long maxSize, bool skipHidden)
        {
            FileFilter f = new FileFilter(minSize, maxSize, skipHidden);
            Assert.False(f.ShouldInclude(isDirectory, length, isHidden));
        }
    }
}