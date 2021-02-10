using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ByteStream.Utils.Unsafe
{
    internal unsafe static class Memory
    {
        /// <summary>
        /// Copies memory between pointers. DOES NOT CHECK BOUNDARIES OR NULL!
        /// </summary>
        /// <param name="source">The source memory.</param>
        /// <param name="sourceIndex">The source index.</param>
        /// <param name="destination">The destination memory.</param>
        /// <param name="destinationIndex">The destination index.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyMemory(void* source, void* destination, int length)
        {
            Buffer.MemoryCopy(source, destination, length, length);
        }

        /// <summary>
        /// Copies memory between pointers. DOES NOT CHECK BOUNDARIES OR NULL!
        /// </summary>
        /// <param name="source">The source memory.</param>
        /// <param name="sourceIndex">The source index.</param>
        /// <param name="destination">The destination memory.</param>
        /// <param name="destinationIndex">The destination index.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyMemory(byte[] source, int sourceIndex, IntPtr destination, int destinationIndex, int length)
        {
            fixed (byte* src = &source[sourceIndex])
            {
                Buffer.MemoryCopy(src, (byte*)destination + destinationIndex, length, length);
            }
        }

        /// <summary>
        /// Copies memory between pointers. DOES NOT CHECK BOUNDARIES OR NULL!
        /// </summary>
        /// <param name="source">The source memory.</param>
        /// <param name="sourceIndex">The source index.</param>
        /// <param name="destination">The destination memory.</param>
        /// <param name="destinationIndex">The destination index.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyMemory(IntPtr source, int sourceIndex, byte[] destination, int destinationIndex, int length)
        {
            fixed (byte* dst = &destination[destinationIndex])
            {
                Buffer.MemoryCopy((byte*)source + sourceIndex, dst, length, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearMemory(IntPtr source, int length)
        {
            ClearMemory((void*)source, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearMemory(void* source, int length)
        {
            long c = length >> 3; // longs

            int i = 0;
            for (; i < c; i++)
                *((ulong*)source + i) = 0;

            i = i << 3;
            for (; i < length; i++)
                *((byte*)source + i) = 0;
        }
    }
}
