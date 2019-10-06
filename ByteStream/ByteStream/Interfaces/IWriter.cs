using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IWriter : IBuffer
    {
        void Write<T>(T value) where T : unmanaged;

        void CopyTo(byte[] buffer);
        void CopyTo(byte[] buffer, int destinationIndex);
        void CopyTo(byte[] buffer, int destinationIndex, int length);
        void CopyTo(IntPtr ptr);
        void CopyTo(IntPtr ptr, int destinationIndex);
        void CopyTo(IntPtr ptr, int destinationIndex, int length);
    }
}
