using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Gets the next power of two of the current number.
        /// </summary>
        public static int NextPowerOfTwo(this int num)
        {
            num--;
            num |= num >> 1;
            num |= num >> 2;
            num |= num >> 4;
            num |= num >> 8;
            num |= num >> 16;
            num++;
            return num;
        }

    }
}
