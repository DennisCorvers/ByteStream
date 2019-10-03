using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IWriter : IBuffer
    {
        void Write<T>(T value) where T : unmanaged;

        void Resize(int newSize);

        void CopyToBytes(byte[] buffer);
        void CopyToBytes(byte[] buffer, int destinationIndex);
        void CopyToBytes(byte[] buffer, int destinationIndex, int length);
        void CopyToPtr(IntPtr ptr);
        void CopyToPtr(IntPtr ptr, int destinationIndex);
        void CopyToPtr(IntPtr ptr, int destinationIndex, int length);
    }
}
