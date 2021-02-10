using ByteStream.Utils;
using ByteStream.Utils.Unsafe;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStream
{
    public unsafe static class BinaryHelper
    {
        /// <summary>
        /// Writes a byte array to the specified array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteBytes(byte[] dest, int offset, byte[] value)
        {
            value.CopyToUnsafe(0, dest, offset, value.Length);
        }

        /// <summary>
        /// Writes a byte array to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteBytes(IntPtr dest, int offset, byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            fixed (byte* src = value)
            {
                Buffer.MemoryCopy(src, (void*)(dest + offset), value.Length, value.Length);
            }
        }

        /// <summary>
        /// Writes a blittable value type to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(byte[] dest, int offset, T value) where T : unmanaged
        {
            fixed (byte* ptr = &dest[offset])
            {
                *(T*)ptr = value;
            }
        }

        /// <summary>
        /// Writes a blittable value type to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(IntPtr dest, int offset, T value) where T : unmanaged
        {
            *(T*)(dest + offset) = value;
        }

        /// <summary>
        /// Writes a blittable value type to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write<T>(void* dest, int offset, T value) where T : unmanaged
        {
            *(T*)((byte*)dest + offset) = value;
        }

        /// <summary>
        /// Reads a byte-memory from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="length">The total length of the byte buffer to read as an int32</param>
        public static byte[] ReadBytes(IntPtr data, int offset, int length)
        {
            if (length < 1)
                return new byte[0];

            byte[] result = new byte[length];

            fixed (byte* ptr = result)
            {
                Buffer.MemoryCopy((byte*)data + offset, ptr, length, length);
            }

            return result;
        }

        /// <summary>
        /// Reads a byte-array from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        /// <param name="length">The total length of the byte buffer to read as an int32</param>
        public static byte[] ReadBytes(byte[] bin, int offset, int length)
        {
            if (length < 1)
                return new byte[0];

            byte[] result = new byte[length];

            bin.CopyToUnsafe(offset, result, 0, length);
            return result;
        }

        /// <summary>
        /// Reads a blittable value type from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(byte[] bin, int offset) where T : unmanaged
        {
            fixed (byte* ptr = &bin[offset])
            {
                return *(T*)ptr;
            }
        }

        /// <summary>
        /// Reads a blittable value type from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(IntPtr data, int offset) where T : unmanaged
        {
            return *(T*)(data + offset);
        }

        /// <summary>
        /// Reads a blittable value type from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Read<T>(void* data, int offset) where T : unmanaged
        {
            return *(T*)((byte*)data + offset);
        }
    }
}
