using ByteStream.Interfaces;
using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Unmanaged
{
    public struct PtrWriter : IWriter<PtrWriter>
    {
        private const int DEFAULTSIZE = 64;

#pragma warning disable IDE0032
        private IntPtr m_buffer;
        private int m_length;
        private int m_offset;
#pragma warning restore IDE0032

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
        /// <summary>
        /// Gets the internal buffer used by this writer.
        /// Do not modify the buffer while performing write operations.
        /// </summary>
        public IntPtr Buffer
            => m_buffer;

        /// <summary>
        /// Creates a new writer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        /// <param name="bufferLength">The amount of bytes that can be written.</param>
        public PtrWriter(IntPtr buffer, int length)
            : this(buffer, length, 0)
        { }
        /// <summary>
        /// Creates a new writer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public PtrWriter(IntPtr buffer, int offset, int length)
        {
            if (buffer == IntPtr.Zero)
            { throw new ArgumentNullException("buffer"); }
            if (offset < 0)
            { throw new ArgumentOutOfRangeException("offset"); }
            if (length < 0)
            { throw new ArgumentOutOfRangeException("count"); }
            if (offset > length)
            { throw new ArgumentException("Offset exceeds buffer length."); }

            m_buffer = buffer;
            m_offset = offset;
            m_length = length;
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
        public PtrWriter Write<T>(T value) where T : unmanaged
        {
            WriteValueInternal(value); return this;
        }

        /// <summary>
        /// Writes a byte array.
        /// </summary>
        /// <param name="includeSize">TRUE to include the size as an uint16</param>
        public void WriteBytes(byte[] value, bool includeSize = false)
        {
            if (includeSize)
            {
                if (value.Length > ushort.MaxValue)
                { throw new ArgumentOutOfRangeException("value", "Maximum size of 65535 exceeded."); }
                Write((ushort)value.Length);
            }

            EnsureCapacity(value.Length);
            BinaryHelper.WriteBytes(m_buffer, m_offset, value);
            m_offset += value.Length;
        }
        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="includeSize">TRUE to include the size as an uint16</param>
        public void WriteUTF16(string value, bool includeSize = false)
        {
            if (includeSize)
            {
                if (value.Length > ushort.MaxValue)
                { throw new ArgumentOutOfRangeException("value", "Maximum size of 65535 exceeded."); }
                Write((ushort)value.Length);
            }

            EnsureCapacity(value.Length * sizeof(char));
            StringHelper.WriteUTF16(m_buffer, m_offset, value);
            m_offset += value.Length * sizeof(char);
        }
        /// <summary>
        /// Writes a string in ANSI encoding. Each character requires 1 byte.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="value"></param>
        public void WriteANSI(string value, bool includeSize = false)
        {
            if (includeSize)
            {
                if (value.Length > ushort.MaxValue)
                { throw new ArgumentOutOfRangeException("value", "Maximum size of 65535 exceeded."); }
                Write((ushort)value.Length);
            }

            EnsureCapacity(value.Length);
            StringHelper.WriteANSI(m_buffer, m_offset, value);
            m_offset += value.Length;
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

            Memory.CopyMemory(m_buffer, 0, buffer, destinationIndex, length);
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

            Memory.CopyMemory(m_buffer, 0, ptr, destinationIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteValueInternal<T>(T value) where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);
            BinaryHelper.Write(m_buffer, m_offset, value);
            m_offset += size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int bytesToAdd)
        {
            if (m_length < m_offset + bytesToAdd)
            { throw new InvalidOperationException("Unable to write more data."); }
        }
    }
}
