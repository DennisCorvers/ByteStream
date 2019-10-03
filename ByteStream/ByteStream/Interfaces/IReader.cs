using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IReader : IBuffer
    {
        T Read<T>() where T : unmanaged;
    }
}
