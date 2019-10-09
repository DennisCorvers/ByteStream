using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IReader : IBuffer
    {
        T Read<T>() where T : unmanaged;
        bool TryRead<T>(out T value) where T : unmanaged;
    }
}
