﻿using ByteStream.Interfaces;
using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Mananged
{
    public struct ByteWriter : IWriter
    {
        private const int DEFAULTSIZE = 64;

#pragma warning disable IDE0032
        private byte[] m_buffer;
        private int m_offset;
        private bool m_isFixedSize;
#pragma warning restore IDE0032

        /// <summary>
        /// The length of this writer.
        /// </summary>
        public int Length
            => m_buffer.Length;
        /// <summary>
        /// The current write offset.
        /// </summary>
        public int Offset
            => m_offset;
        /// <summary>
        /// Determines if this writer has a fixed size.
        /// </summary>
        public bool IsFixedSize
            => m_isFixedSize;
        /// <summary>
        /// Gets the internal buffer used by this writer.
        /// Do not modify the buffer while performing write operations.
        /// </summary>
        public byte[] Buffer
            => m_buffer;

        /// <summary>
        /// Creates a new instance of bytewriter with an empty byffer.
        /// </summary>
        /// <param name="initialSize">The initial size of the buffer.</param>
        /// <param name="isFixedSize">Determines if the buffer is allowed to increase its size automatically.</param>
        public ByteWriter(int initialSize, bool isFixedSize)
        {
            if (initialSize < 1) { throw new ArgumentException("initialSize"); }

            m_buffer = new byte[initialSize];
            m_isFixedSize = isFixedSize;
            m_offset = 0;
        }
        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="data">The buffer to use with this writer.</param>
        public ByteWriter(byte[] data) : this(data, true)
        { }
        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        /// <param name="isFixedSize">Determines if the buffer is allowed to increase its size automatically.</param>
        public ByteWriter(byte[] data, bool isFixedSize)
        {
            m_buffer = data ?? throw new ArgumentNullException("buffer");
            m_isFixedSize = isFixedSize;
            m_offset = 0;
        }

        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="data">The byte array to wrap.</param>
        /// <param name="offset">The write offset.</param>
        public ByteWriter(byte[] data, int offset)
        {
            m_buffer = data ?? throw new ArgumentNullException("data");

            if (offset < 0)
            { throw new ArgumentOutOfRangeException("offset"); }
            if (offset > data.Length)
            { throw new ArgumentException("Offset is larger than array length."); }

            m_isFixedSize = true;
            m_offset = offset;
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
        /// Resizes the current buffer to the current write offset.
        /// Performs an array copy.
        /// </summary>
        public void Trim()
        {
            ArrayExtensions.ResizeUnsafe(ref m_buffer, m_offset);
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
        public void Write<T>(T value) where T : unmanaged
        {
            WriteValueInternal(value);
        }

        /// <summary>
        /// Writes a byte array. Does NOT include the length.
        /// </summary>
        /// <param name="value"></param>
        public void WriteBytes(byte[] value)
        {
            EnsureCapacity(value.Length);
            BinaryHelper.WriteBytes(m_buffer, m_offset, value);
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
            StringHelper.WriteUTF16(m_buffer, m_offset, value);
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
            StringHelper.WriteANSI(m_buffer, m_offset, value);
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
        /// Writes a string in UTF8 encoding. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUTF8(string value)
        {
            int byteSize = Encoding.UTF8.GetByteCount(value);

            EnsureCapacity(byteSize + sizeof(ushort));
            BinaryHelper.Write(m_buffer, m_offset, (ushort)value.Length);
            m_offset += sizeof(ushort);

            StringHelper.WriteUTF8(m_buffer, m_offset, value);
            m_offset += byteSize;
        }

        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(byte[] buffer)
        {
            if (buffer == null)
            { throw new ArgumentNullException("buffer"); }

            if (buffer.Length < m_offset)
            { throw new ArgumentOutOfRangeException("destinationIndex", "Copy action exceeds the supplied buffer!"); }

            m_buffer.CopyToUnsafe(0, buffer, 0, m_offset);
        }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(byte[] buffer, int destinationIndex)
        {
            if (buffer == null)
            { throw new ArgumentNullException("buffer"); }

            if (buffer.Length < destinationIndex + m_offset)
            { throw new ArgumentOutOfRangeException("destinationIndex", "Copy action exceeds the supplied buffer!"); }

            if (destinationIndex < 0)
            { throw new ArgumentOutOfRangeException("destinationIndex"); }

            m_buffer.CopyToUnsafe(0, buffer, destinationIndex, m_offset);
        }
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

            m_buffer.CopyToUnsafe(0, buffer, destinationIndex, length);
        }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            { throw new ArgumentNullException("buffer"); }

            Memory.CopyMemory(m_buffer, m_offset, ptr, 0, m_offset);
        }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(IntPtr ptr, int destinationIndex)
        {
            if (ptr == IntPtr.Zero)
            { throw new ArgumentNullException("buffer"); }

            if (destinationIndex < 0)
            { throw new ArgumentOutOfRangeException("destinationIndex"); }

            Memory.CopyMemory(m_buffer, m_offset, ptr, destinationIndex, m_offset);
        }
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

            Memory.CopyMemory(m_buffer, m_offset, ptr, destinationIndex, length);
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
            int newAmount = m_offset + bytesToAdd;
            if (newAmount > Length)
            { ResizeInternal(newAmount); }
        }
        private void ResizeInternal(int resizeTo)
        {
            if (m_isFixedSize)
            { throw new InvalidOperationException("Buffer is set to a fixed size and cannot resize automatically!"); }

            int nextpwr = Length.NextPowerOfTwo();
            int newSize = Length == nextpwr ? Length * 2 : nextpwr;

            ArrayExtensions.ResizeUnsafe(ref m_buffer, newSize);
        }
    }
}
