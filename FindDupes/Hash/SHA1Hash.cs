using System;
using System.Security.Cryptography;
using FindDupes.Abstracts;

namespace FindDupes.Hash
{
    public class SHA1Hash : IFileHash
    {
        private readonly SHA1 _hasher;

        public SHA1Hash()
        {
            _hasher = SHA1.Create();
        }

        public ulong GetHash(in ReadOnlySpan<byte> data)
        {
            return BitConverter.ToUInt64(_hasher.ComputeHash(data.ToArray()));
        }
    }
}