using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IWriter : IBuffer
    {
        void Write<T>(T value) where T : unmanaged;
        bool TryWrite<T>(T value) where T : unmanaged;
    }
}
