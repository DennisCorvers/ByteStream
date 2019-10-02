using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IBuffer
    {
        int Length { get; }
        int Offset { get; }
        bool IsFixedSize { get; }

        void SkipBytes(int amount);
        void Resize(int newSize);
        void Clear();

        void CopyToBytes(byte[] buffer);
        void CopyToBytes(byte[] buffer, int destinationIndex);
        void CopyToBytes(byte[] buffer, int destinationIndex, int length);
        void CopyToPtr(IntPtr ptr);
        void CopyToPtr(IntPtr ptr, int destinationIndex);
        void CopyToPtr(IntPtr ptr, int destinationIndex, int length);
    }
}
