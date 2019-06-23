using System;
using System.Collections.Generic;
using Xunit;

namespace FindDupes.Tests
{
    public class FileDetailsComparerTests
    {
        [Theory]
        [MemberData(nameof(TrueComparerData))]
        public void FileDetailsEqualsTest(FileDetails a, FileDetails b)
        {
            FileDetailsComparer c = new FileDetailsComparer();
            Assert.Equal(a, b, c);
        }

        [Theory]
        [MemberData(nameof(FalseComparerData))]
        public void FileDetailsNotEqualsTest(FileDetails a, FileDetails b)
        {
            FileDetailsComparer c = new FileDetailsComparer();
            Assert.NotEqual(a, b, c);
        }

        [Theory]
        [MemberData(nameof(TrueComparerData))]
        public void FileDetailsHashCodeTest(FileDetails a, FileDetails b)
        {
            FileDetailsComparer c = new FileDetailsComparer();
            Assert.Equal(c.GetHashCode(a), c.GetHashCode(b));
        }

        public static IEnumerable<object[]> FalseComparerData()
        {
            DateTimeOffset one = DateTimeOffset.FromUnixTimeMilliseconds(1);
            DateTimeOffset two = DateTimeOffset.FromUnixTimeMilliseconds(2);

            //Filename and size should be ignored
            yield return new object[] { new FileDetails("filename", 1, one, 1), new FileDetails("filename2", 2, two, 1) };
            yield return new object[] { new FileDetails("filename", 1, one, 1), new FileDetails("filename2", 2, two, 2) };
        }

        public static IEnumerable<object[]> TrueComparerData()
        {
            DateTimeOffset one = DateTimeOffset.FromUnixTimeMilliseconds(1);

            //Filename and size should be ignored
            yield return new object[] { new FileDetails("filename", 1, one, 1), new FileDetails("filename2", 2, one, 1) };
        }
    }
}
