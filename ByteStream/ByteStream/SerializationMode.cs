using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream
{
    public enum SerializationMode : byte
    {
        None = 0,
        Reading = 1,
        Writing = 2
    }
}
