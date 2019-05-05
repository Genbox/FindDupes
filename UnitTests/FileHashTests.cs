using System.Text;
using FindDupes.Hash;
using FindDupes.Interfaces;
using Xunit;

namespace UnitTests
{
    public class FileHashTests
    {
        [Theory]
        [InlineData("Hello World", 10764455564493180894)]
        [InlineData("\0", 11145182160106660097)]
        [InlineData("1", 15532033055997473560)]
        public void xxHash64(string a, ulong hash)
        {
            IFileHash h = new xxHash();
            Assert.Equal(hash, h.GetHash(Encoding.Unicode.GetBytes(a)));
        }

        [Theory]
        [InlineData("Hello World", 5750913225038017743)]
        [InlineData("\0", 3001610411640981780)]
        [InlineData("1", 4397373564687294441)]
        public void Sha1(string a, ulong hash)
        {
            IFileHash h = new SHA1Hash();
            Assert.Equal(hash, h.GetHash(Encoding.Unicode.GetBytes(a)));
        }
    }
}