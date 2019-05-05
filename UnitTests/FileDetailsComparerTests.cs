using System;
using System.Collections.Generic;
using FindDupes;
using FindDupes.Structs;
using Xunit;

namespace UnitTests
{
    public class FileDetailsComparerTests
    {
        [Theory]
        [MemberData(nameof(ComparerData))]
        public void FileDetailsEqualsTest(bool useSize, bool useTimestamp, bool useHash, FileDetails a, FileDetails b)
        {
            FileDetailsComparer c = new FileDetailsComparer(useSize, useTimestamp, useHash);
            Assert.Equal(a, b, c);
        }

        [Theory]
        [MemberData(nameof(ComparerData))]
        public void FileDetailsHashCodeTest(bool useSize, bool useTimestamp, bool useHash, FileDetails a, FileDetails b)
        {
            FileDetailsComparer c = new FileDetailsComparer(useSize, useTimestamp, useHash);
            Assert.Equal(c.GetHashCode(a), c.GetHashCode(b));
        }

        public static IEnumerable<object[]> ComparerData()
        {
            DateTimeOffset one = DateTimeOffset.FromUnixTimeMilliseconds(1);
            DateTimeOffset two = DateTimeOffset.FromUnixTimeMilliseconds(2);

            yield return new object[] { true, true, true, new FileDetails("filename", 1, one, 1), new FileDetails("filename", 1, one, 1) };
            yield return new object[] { false, true, true, new FileDetails("filename", 1, one, 1), new FileDetails("filename", 2, one, 1) };
            yield return new object[] { false, false, true, new FileDetails("filename", 1, one, 1), new FileDetails("filename", 2, two, 1) };
            yield return new object[] { false, false, false, new FileDetails("filename", 1, one, 1), new FileDetails("filename", 2, two, 2) };
        }
    }
}
