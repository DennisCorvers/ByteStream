using ByteStream.Interfaces;
using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStream.Unmanaged
{
    public struct ByteWriterFast : IWriter<ByteWriterFast>, IDisposable
    {
        private const int DEFAULTSIZE = 64;

#pragma warning disable IDE0032
        private GCHandle m_pinnedArray;
        private int m_offset;
        private int m_length;
        private byte[] m_buffer;
#pragma warning restore IDE0032

        private IntPtr m_ptr
            => m_pinnedArray.AddrOfPinnedObject();

        /// <summary>
        /// The length of this writer.
        /// </summary>
        public int Length
            => m_length;
        /// <summary>
        /// The current write offset.
        /// </summary>
        public int Offset
            => m_offset;
        public bool IsPinned
            => m_pinnedArray.IsAllocated;

        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="data">The buffer to use with this writer.</param>
        public ByteWriterFast(byte[] data)
        {
            m_buffer = data ?? throw new ArgumentNullException("data");
            m_offset = 0;
            m_length = data.Length;
        }

        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="data">The byte array to wrap.</param>
        /// <param name="offset">The write offset.</param>
        public ByteWriterFast(byte[] data, int offset, int length)
        {
            if (data == null) { throw new ArgumentNullException("data"); }

            if (offset < 0)
            { throw new ArgumentOutOfRangeException("offset"); }
            if (length < 0)
            { throw new ArgumentOutOfRangeException("count"); }
            if (data.Length - offset < length)
            { throw new ArgumentException("Array out of bounds."); }

            m_buffer = data;
            m_offset = offset;
            m_length = length;
        }

        /// <summary>
        /// Pins the underlaying buffer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pin()
        {
            if (!IsPinned)
            { m_pinnedArray = GCHandle.Alloc(m_buffer, GCHandleType.Pinned); }
        }

        /// <summary>
        /// Frees the underlaying buffer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnPin()
        {
            if (IsPinned)
            { m_pinnedArray.Free(); }
        }

        /// <summary>
        /// Increases the write offset by some amount.
        /// </summary>
        /// <param name="amount">The amounts of bytes to skip.</param>
        public void SkipBytes(int amount)
        {
            if (amount < 1)
            { throw new ArgumentOutOfRangeException("amount"); }

            EnsureCapacity(amount);
            m_offset += amount;
        }
        /// <summary>
        /// Resets the offset to zero.
        /// </summary>
        public void Clear()
        {
            m_offset = 0;
        }

        /// <summary>
        /// Writes a blittable struct or primitive value to the buffer.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        public ByteWriterFast Write<T>(T value) where T : unmanaged
        {
            WriteValueInternal(value); return this;
        }

        /// <summary>
        /// Writes a byte array. Does NOT include the length.
        /// </summary>
        /// <param name="value"></param>
        public void WriteBytes(byte[] value)
        {
            EnsureCapacity(value.Length);
            BinaryHelper.WriteBytes(m_ptr, m_offset, value);
            m_offset += value.Length;
        }
        /// <summary>
        /// Writes a byte array. Includes the length as uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteBytesLength(byte[] value)
        {
            Write((ushort)value.Length);
            WriteBytes(value);
        }

        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUTF16(string value)
        {
            EnsureCapacity(value.Length * sizeof(char));
            StringHelper.WriteUTF16(m_ptr, m_offset, value);
            m_offset += value.Length * sizeof(char);
        }
        /// <summary>
        /// Writes a string as a double-byte character set. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUTF16Length(string value)
        {
            Write((ushort)value.Length);
            WriteUTF16(value);
        }
        /// <summary>
        /// Writes a string in ANSI encoding. Each character requires 1 byte.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="value"></param>
        public void WriteANSI(string value)
        {
            EnsureCapacity(value.Length);
            StringHelper.WriteANSI(m_ptr, m_offset, value);
            m_offset += value.Length;
        }
        /// <summary>
        /// Writes a string in ANSI encoding. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteANSILength(string value)
        {
            Write((ushort)value.Length);
            WriteANSI(value);
        }

        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(byte[] buffer)
        { CopyTo(buffer, 0, m_offset); }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(byte[] buffer, int destinationIndex)
        { CopyTo(buffer, destinationIndex, m_offset); }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        /// <param name="length">The total length to copy (starting from 0)</param>
        public void CopyTo(byte[] buffer, int destinationIndex, int length)
        {
            if (buffer == null)
            { throw new ArgumentNullException("buffer"); }

            if (buffer.Length < destinationIndex + length)
            { throw new ArgumentOutOfRangeException("destinationIndex", "Copy action exceeds the supplied buffer!"); }

            if (destinationIndex < 0)
            { throw new ArgumentOutOfRangeException("destinationIndex"); }

            if (length < 1 || length > Length)
            { throw new ArgumentOutOfRangeException("length"); }

            Memory.CopyMemory(m_ptr, 0, buffer, destinationIndex, length);
        }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(IntPtr ptr)
        { CopyTo(ptr, 0, m_offset); }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(IntPtr ptr, int destinationIndex)
        { CopyTo(ptr, destinationIndex, m_offset); }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        /// <param name="length">The total length to copy (starting from 0)</param>
        public void CopyTo(IntPtr ptr, int destinationIndex, int length)
        {
            if (ptr == IntPtr.Zero)
            { throw new ArgumentNullException("buffer"); }

            if (destinationIndex < 0)
            { throw new ArgumentOutOfRangeException("destinationIndex"); }

            if (length < 1 || length > Length)
            { throw new ArgumentOutOfRangeException("length"); }

            Memory.CopyMemory(m_ptr, 0, ptr, destinationIndex, length);
        }

        /// <summary>
        /// Frees the pinned array.
        /// </summary>
        public void Dispose()
        { UnPin(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteValueInternal<T>(T value) where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);
            BinaryHelper.Write(m_ptr, m_offset, value);
            m_offset += size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int bytesToAdd)
        {
            if (!IsPinned)
            { throw new InvalidOperationException("Call Pin before performing write operations."); }

            if (m_length < m_offset + bytesToAdd)
            { throw new InvalidOperationException("Unable to write more data."); }
        }
    }
}
