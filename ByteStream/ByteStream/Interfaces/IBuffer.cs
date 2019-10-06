using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IBuffer
    {
        int Length { get; }
        int Offset { get; }

        void SkipBytes(int amount);
        void Clear();
    }
}
