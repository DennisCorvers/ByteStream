using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IReader : IBuffer
    {
        T ReadValue<T>() where T : unmanaged;
    }
}
