using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream
{
    public static class Memory
    {
        public unsafe static void CopyUnsafe<T>(this T[] source, int sourceIndex,
            T[] destination, int destinationIndex, int length) where T : unmanaged
        {
            length *= sizeof(T);

            fixed (T* src = &source[sourceIndex * sizeof(T)])
            fixed (T* dst = &destination[destinationIndex * sizeof(T)])
            { Buffer.MemoryCopy(src, dst, length, length); }
        }
    }
}
