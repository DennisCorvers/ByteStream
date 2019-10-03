﻿using ByteStream.Interfaces;
using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Mananged
{
    public class ByteWriterLight : IWriter
    {
        private const int DEFAULTSIZE = 64;

#pragma warning disable IDE0032
        protected byte[] m_buffer;
        protected int m_offset;
        protected bool m_isFixedSize;
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
        /// Creates a new instance of bytewriter with an empty buffer.
        /// </summary>
        public ByteWriterLight() : this(DEFAULTSIZE, true)
        { }
        /// <summary>
        /// Creates a new instance of bytewriter with an empty byffer.
        /// </summary>
        /// <param name="initialSize">The initial size of the buffer.</param>
        /// <param name="isFixedSize">Determines if the buffer is allowed to increase its size automatically.</param>
        public ByteWriterLight(int initialSize, bool isFixedSize)
        {
            m_buffer = new byte[initialSize.NextPowerOfTwo()];
            m_isFixedSize = isFixedSize;
            m_offset = 0;
        }
        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        public ByteWriterLight(byte[] buffer) : this(buffer, true)
        { }
        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        /// <param name="isFixedSize">Determines if the buffer is allowed to increase its size automatically.</param>
        public ByteWriterLight(byte[] buffer, bool isFixedSize)
        {
            m_buffer = buffer ?? throw new ArgumentNullException("buffer");
            m_isFixedSize = isFixedSize;
            m_offset = 0;
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
        /// Resizes the current buffer.
        /// Performs an array copy.
        /// </summary>
        /// <param name="newSize">The new size of the buffer.</param>
        public void Resize(int newSize)
        {
            if (newSize < 0)
            { throw new ArgumentOutOfRangeException("newSize"); }
            if (newSize < m_offset)
            { throw new InvalidOperationException("New size is smaller than the current write offset!"); }

            ArrayExtensions.ResizeUnsafe(ref m_buffer, newSize);
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
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyToBytes(byte[] buffer)
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
        public void CopyToBytes(byte[] buffer, int destinationIndex)
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
        public void CopyToBytes(byte[] buffer, int destinationIndex, int length)
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
        public void CopyToPtr(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            { throw new ArgumentNullException("buffer"); }

            Memory.CopyMemory(m_buffer, m_offset, ptr, 0, m_offset);
        }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyToPtr(IntPtr ptr, int destinationIndex)
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
        public void CopyToPtr(IntPtr ptr, int destinationIndex, int length)
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
            BinaryHelper.WriteValue(m_buffer, m_offset, value);
            m_offset += size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int bytesToAdd)
        {
            int newAmount = m_offset + bytesToAdd;
            if (newAmount > Length)
            { Resize(newAmount); }
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
