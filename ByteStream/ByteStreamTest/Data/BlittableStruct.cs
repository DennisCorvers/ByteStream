using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamTest.Data
{
    [StructLayout(LayoutKind.Explicit, Size = 11)]
    public struct BlittableStruct
    {
        [FieldOffset(0)]
        public double ValOne;
        [FieldOffset(8)]
        public bool ValTwo;
        [FieldOffset(9)]
        public short ValThree;

        public bool IsEqual(BlittableStruct other)
        {
            return ValOne == other.ValOne && ValTwo == other.ValTwo && ValThree == other.ValThree;
        }
    }
}
