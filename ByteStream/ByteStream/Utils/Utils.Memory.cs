using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStream.Utils
{
    public static class Memory
    {
        /// <summary>
        /// Allocates memory using Marshal
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr Alloc(int size)
        {
            return Marshal.AllocHGlobal(size);
        }
        /// <summary>
        /// Deallocates memory using Marshal
        /// </summary>
        /// <param name="ptr"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Dealloc(IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }
        /// <summary>
        /// Resizes a pointer using Marshal alloc.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr ResizePtr(IntPtr ptr, int newSize)
        {
            return Marshal.ReAllocHGlobal(ptr, (IntPtr)newSize);
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
        public unsafe static void CopyMemory(IntPtr source, int sourceIndex, IntPtr destination, int destinationIndex, int length)
        {
            Buffer.MemoryCopy(
                (source + sourceIndex).ToPointer(),
                (destination + destinationIndex).ToPointer(),
                length, length);
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
        public unsafe static void CopyMemory(byte[] source, int sourceIndex, IntPtr destination, int destinationIndex, int length)
        {
            fixed (byte* src = &source[sourceIndex])
            { Buffer.MemoryCopy(src, (destination + destinationIndex).ToPointer(), length, length); }
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
        public unsafe static void CopyMemory(IntPtr source, int sourceIndex, byte[] destination, int destinationIndex, int length)
        {
            fixed (byte* dst = &destination[destinationIndex])
            { Buffer.MemoryCopy((source + sourceIndex).ToPointer(), dst, length, length); }
        }
    }
}
