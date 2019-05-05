using System;

namespace FindDupes.Interfaces
{
    public interface IFileHash
    {
        ulong GetHash(in ReadOnlySpan<byte> data);
    }
}
