using ByteStream.Interfaces;
using ByteStream.Utils;
using ByteStream.Utils.Unsafe;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Unmanaged
{
    public struct PtrWriter : IWriter
    {
#pragma warning disable IDE0032
        private readonly IntPtr m_buffer;
        private readonly int m_length;
        private int m_offset;
#pragma warning restore IDE0032

        /// <summary>
        /// The length of the <see cref="PtrWriter"/>.
        /// </summary>
        public int Length
            => m_length;
        /// <summary>
        /// The current write offset.
        /// </summary>
        public int Offset
            => m_offset;
        /// <summary>
        /// Gets the internal buffer used by the <see cref="PtrWriter"/>.
        /// Do not modify the buffer while performing write operations.
        /// </summary>
        public IntPtr Buffer
            => m_buffer;

        /// <summary>
        /// Creates a new <see cref="PtrWriter"/>.
        /// </summary>
        /// <param name="buffer">The buffer to use with this <see cref="PtrWriter"/>.</param>
        /// <param name="bufferLength">The amount of bytes that can be written.</param>
        public PtrWriter(IntPtr buffer, int length)
            : this(buffer, 0, length)
        { }

        /// <summary>
        /// Creates a new <see cref="PtrWriter"/>.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public PtrWriter(IntPtr buffer, int offset, int length)
        {
            if (buffer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(buffer));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if ((uint)offset > length)
                throw new ArgumentOutOfRangeException("Offset exceeds buffer length.");

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
                throw new ArgumentOutOfRangeException(nameof(amount));

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
        /// Reserves 4-bytes of space for a size value at the start of the <see cref="PtrWriter"/>.
        /// </summary>
        public void ReserveSizePrefix()
        {
            EnsureCapacity(4);
            m_offset += 4;
        }

        /// <summary>
        /// Writes the total size at the start of the <see cref="PtrWriter"/> as an <see cref="int"/>.
        /// </summary>
        public int PrefixSize()
        {
            BinaryHelper.Write(m_buffer, 0, m_offset);
            return m_offset;
        }


        /// <summary>
        /// Writes a blittable struct or primitive value to the <see cref="PtrWriter"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        public void Write<T>(T value) where T : unmanaged
        {
            unsafe
            {
                int size = sizeof(T);
                EnsureCapacity(size);
                BinaryHelper.Write(m_buffer, m_offset, value);
                m_offset += size;
            }
        }

        /// <summary>
        /// Tries to write a blittable struct or primitive value to the <see cref="PtrWriter"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        /// <returns>Returns false if the value couldn't be written.</returns>
        public bool TryWrite<T>(T value) where T : unmanaged
        {
            unsafe
            {
                int size = sizeof(T);
                if (m_offset + size > m_length) { return false; }
                BinaryHelper.Write(m_buffer, m_offset, value);
                m_offset += size;
            }
            return true;
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
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum size of 65.535 exceeded.");
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
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum size of 65.535 exceeded.");
                Write((ushort)value.Length);
            }

            EnsureCapacity(value.Length * sizeof(char));
            StringHelper.WriteUTF16(m_buffer, m_offset, value);
            m_offset += value.Length * sizeof(char);
        }

        /// <summary>
        /// Writes a string in ANSI encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="value"></param>
        public void WriteANSI(string value, bool includeSize = false)
        {
            if (includeSize)
            {
                if (value.Length > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum size of 65.535 exceeded.");
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
        {
            CopyTo(buffer, 0, m_offset);
        }

        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(byte[] buffer, int destinationIndex)
        {
            CopyTo(buffer, destinationIndex, m_offset);
        }

        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        /// <param name="length">The total length to copy (starting from 0)</param>
        public void CopyTo(byte[] buffer, int destinationIndex, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if ((uint)(destinationIndex + length) > buffer.Length)
                throw new ArgumentOutOfRangeException("Copy action exceeds the supplied buffer!");

            if ((uint)length > Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            Memory.CopyMemory(m_buffer, 0, buffer, destinationIndex, length);
        }

        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(IntPtr ptr)
        {
            CopyTo(ptr, 0, m_offset);
        }

        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyTo(IntPtr ptr, int destinationIndex)
        {
            CopyTo(ptr, destinationIndex, m_offset);
        }

        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        /// <param name="length">The total length to copy (starting from 0)</param>
        public void CopyTo(IntPtr ptr, int destinationIndex, int length)
        {
            if (ptr == IntPtr.Zero)
            { throw new ArgumentNullException(nameof(ptr)); }

            if (destinationIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));

            if ((uint)length > Length)
            { throw new ArgumentOutOfRangeException(nameof(length)); }

            Memory.CopyMemory(m_buffer, 0, ptr, destinationIndex, length);
        }

        private void EnsureCapacity(int bytesToAdd)
        {
            if (m_length < m_offset + bytesToAdd)
                throw new InvalidOperationException("Unable to write more data.");
        }
    }
}
