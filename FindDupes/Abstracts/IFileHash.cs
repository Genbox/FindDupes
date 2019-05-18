using System;

namespace FindDupes.Abstracts
{
    public interface IFileHash
    {
        ulong GetHash(in ReadOnlySpan<byte> data);
    }
}