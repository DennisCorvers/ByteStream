using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamTest.Data
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BlittableStruct
    {
        public double ValOne;
        public byte ValTwo;
        public ushort ValThree;

        public bool IsEqual(BlittableStruct other)
        {
            return ValOne == other.ValOne && ValTwo == other.ValTwo && ValThree == other.ValThree;
        }
    }
}
