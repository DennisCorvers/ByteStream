using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IWriter : IBuffer
    {
        void WriteValue<T>(T value) where T : unmanaged;
    }
}
