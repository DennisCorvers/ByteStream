using ByteStream.Mananged;
using ByteStream.Unmanaged;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStream.Utils.Unsafe
{
    public static unsafe class UnsafeExtensions
    {
        /// <summary>
        /// Copies memory to the writer.
        /// </summary>
        /// <param name="data">The pointer to the data to write.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        public static void WritePtr(ref this ByteWriter writer, void* data, int length)
        {
            writer.SkipBytes(length);

            fixed (byte* ptr = &writer.Buffer[writer.Offset - length])
            { Buffer.MemoryCopy(data, ptr, length, length); }
        }
        /// <summary>
        /// Copies memory to the writer.
        /// </summary>
        /// <param name="data">The pointer to the data to write.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        public static void WritePtr(ref this PtrWriter writer, void* data, int length)
        {
            writer.SkipBytes(length);

            Buffer.MemoryCopy((void*)(writer.Buffer + writer.Offset - length), data, length, length);
        }

        /// <summary>
        /// Copies data from the writer to a buffer.
        /// </summary>
        /// <param name="buffer">The pointer to the buffer position.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        public static void ReadPtr(ref this ByteReader reader, void* buffer, int length)
        {
            reader.SkipBytes(length);

            fixed (byte* ptr = &reader.Buffer[reader.Offset - length])
            { Buffer.MemoryCopy(ptr, buffer, length, length); }
        }
        /// <summary>
        /// Copies data from the writer to a buffer.
        /// </summary>
        /// <param name="buffer">The pointer to the buffer position.</param>
        /// <param name="length">The amount of bytes to copy.</param>
        public static void ReadPtr(ref this PtrReader reader, void* buffer, int length)
        {
            reader.SkipBytes(length);

            Buffer.MemoryCopy((void*)(reader.Buffer + reader.Offset - length), buffer, length, length);
        }

        /// <summary>
        /// Converts the writer to a pointer.
        /// Allocates memory using Marshal.AllocHGlobal
        /// </summary>
        public static PtrWriter* AsPointer(this PtrWriter value)
        {
            PtrWriter* ptr = (PtrWriter*)Marshal.AllocHGlobal(sizeof(PtrWriter));
            *ptr = value; return ptr;
        }
        /// <summary>
        /// Converts the writer to a pointer.
        /// Allocates memory using Marshal.AllocHGlobal
        /// </summary>
        public static PtrReader* AsPointer(this PtrReader value)
        {
            PtrReader* ptr = (PtrReader*)Marshal.AllocHGlobal(sizeof(PtrReader));
            *ptr = value; return ptr;
        }
    }
}
