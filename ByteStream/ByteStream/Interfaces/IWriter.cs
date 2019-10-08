using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IWriter<Y> : IBuffer
    {
        Y Write<T>(T value) where T : unmanaged;
    }
}
