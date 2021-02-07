using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream.Utils
{
    internal class MathUtils
    {
        /// <summary>
        /// Gets the next power of two of the current number.
        /// </summary>
        public static int NextPowerOfTwo(int num)
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
