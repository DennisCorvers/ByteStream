using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStream.Utils
{
    internal static class ArrayExtensions
    {
        /// <summary>
        /// Copies the source array to another. DOES NOT PERFORM BOUNDARY OR NULL CHECKS!
        /// </summary>
        /// <param name="sourceIndex">The source index.</param>
        /// <param name="destination">The destination array.</param>
        /// <param name="destinationIndex">The destination index.</param>
        /// <param name="length">The amount of elements to copy.</param>
        public unsafe static void CopyToUnsafe<T>(this T[] source, int sourceIndex, T[] destination, int destinationIndex, int length)
            where T : unmanaged
        {
            length *= sizeof(T);

            fixed (T* src = &source[sourceIndex])
            fixed (T* dst = &destination[destinationIndex])
            {
                Buffer.MemoryCopy(src, dst, length, length);
            }
        }

        /// <summary>
        /// Duplicates an array.
        /// </summary>
        public unsafe static T[] CloneUnsafe<T>(this T[] source)
            where T : unmanaged
        {
            T[] val = new T[source.Length];
            CopyToUnsafe(source, 0, val, 0, source.Length);
            return val;
        }

        /// <summary>
        /// Resizes an array.
        /// </summary>
        /// <param name="source">The array to resize.</param>
        /// <param name="newSize">The new size of the array.</param>
        public static void ResizeUnsafe<T>(ref T[] source, int newSize) where T : unmanaged
        {
            T[] lArray = source;
            if (lArray == null)
            {
                source = new T[newSize];
                return;
            }

            if (lArray.Length != newSize)
            {
                T[] newArray = new T[newSize];
                newSize = Math.Min(lArray.Length, newSize);
                CopyToUnsafe(lArray, 0, newArray, 0, newSize);

                source = newArray;
            }
        }
    }
}
