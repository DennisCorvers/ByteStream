using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Interfaces
{
    public interface IByteStream
    {
        int Offset { get; }
        int Length { get; }

        SerializationMode Mode { get; }

        void Serialize<T>(ref T value) where T : unmanaged;
        void SerializeString(ref string value, Encoding encoding);
    }
}
