﻿using ByteStream.Interfaces;
using ByteStream.Utils;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Unmanaged
{
    public struct PtrReader : IReader
    {
#pragma warning disable IDE0032
        private readonly IntPtr m_buffer;
        private readonly int m_length;
        private int m_offset;
#pragma warning restore IDE0032

        /// <summary>
        /// The length of this <see cref="PtrReader"/>.
        /// </summary>
        public int Length
            => m_length;
        /// <summary>
        /// The current read offset.
        /// </summary>
        public int Offset
            => m_offset;
        /// <summary>
        /// Gets the internal buffer used by the <see cref="PtrReader"/>.
        /// Do not modify the buffer while performing read operations.
        /// </summary>
        public IntPtr Buffer
            => m_buffer;

        /// <summary>
        /// Creates a new <see cref="PtrReader"/>.
        /// </summary>
        /// <param name="buffer">The data to be read by the <see cref="PtrReader"/>.</param>
        /// <param name="bufferLength">The amount of bytes that can be read.</param>
        public PtrReader(IntPtr buffer, int length)
            : this(buffer, 0, length)
        { }

        /// <summary>
        /// Creates a new <see cref="PtrReader"/>.
        /// </summary>
        /// <param name="buffer">The data to be read by the <see cref="PtrReader"/>.</param>
        /// <param name="length">The amount of bytes that can be read.</param>
        /// <param name="offset">The read offset.</param>
        public PtrReader(IntPtr buffer, int offset, int length)
        {
            if (buffer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(buffer));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if ((uint)offset > length)
            { throw new ArgumentOutOfRangeException("Out of bounds."); }

            m_offset = offset;
            m_length = length;
            m_buffer = buffer;
        }


        /// <summary>
        /// Increases the read offset by some amount.
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
        /// Reads a byte-array from the <see cref="PtrReader"/>. Length is automatically read as an uint16.
        /// </summary>
        public byte[] ReadBytes()
        {
            ushort length = Read<ushort>();
            return ReadBytes(length);
        }

        /// <summary>
        /// Reads a byte-array from the <see cref="PtrReader"/>.
        /// Does NOT automatically read length.
        /// </summary>
        /// <param name="length">The length of the byte-array.</param>
        /// <returns></returns>
        public byte[] ReadBytes(int length)
        {
            EnsureCapacity(length);
            byte[] val = BinaryHelper.ReadBytes(m_buffer, m_offset, length);

            m_offset += length;
            return val;
        }


        /// <summary>
        /// Reads a string as a double-byte character set. Length is automatically retrieved as an uint16.
        /// </summary>
        public string ReadUTF16()
        {
            ushort length = Read<ushort>();
            return ReadUTF16(length);
        }

        /// <summary>
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// Does NOT automatically read length.
        /// </summary>
        /// 
        public string ReadUTF16(int stringLength)
        {
            EnsureCapacity(stringLength * sizeof(char));
            string val = StringHelper.ReadUTF16(m_buffer, m_offset, stringLength);

            m_offset += stringLength * sizeof(char);
            return val;
        }


        /// <summary>
        /// Reads a string in ANSI encoding. Length is automatically retrieved as an uint16.
        /// </summary>
        public string ReadANSI()
        {
            ushort length = Read<ushort>();
            return ReadANSI(length);
        }

        /// <summary>
        /// Reads a string in ANSI encoding. Each character requires 1 byte.
        /// Does NOT automatically read length.
        /// </summary>
        public string ReadANSI(int stringLength)
        {
            EnsureCapacity(stringLength);
            string val = StringHelper.ReadANSI(m_buffer, m_offset, stringLength);

            m_offset += stringLength;
            return val;
        }

        /// <summary>
        /// Reads a string from the <see cref="ByteReader"/>.
        /// Length is automatically retrieved.
        /// </summary>
        /// <returns></returns>
        public string ReadString(Encoding encoding)
        {
            int byteCount = Read<ushort>();

            EnsureCapacity(byteCount);
            return StringHelper.ReadString(m_buffer, m_offset, byteCount, encoding);
        }

        /// <summary>
        /// Reads a blittable struct or primitive value from the <see cref="PtrReader"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        public T Read<T>() where T : unmanaged
        {
            return ReadValueInternal<T>();
        }

        /// <summary>
        /// Tries to read a blittable struct or primitive value from the <see cref="PtrReader"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        /// <returns>Returns false if the value couldn't be read.</returns>
        public bool TryRead<T>(out T value) where T : unmanaged
        {
            value = default;

            unsafe
            {
                int size = sizeof(T);
                if (m_offset + size > m_length) { return false; }
                value = BinaryHelper.Read<T>(m_buffer, m_offset);
                m_offset += size;
            }
            return true;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe T ReadValueInternal<T>() where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);
            T val = BinaryHelper.Read<T>(m_buffer, m_offset);

            m_offset += size;
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int bytesToRead)
        {
            if (bytesToRead + m_offset > m_length)
                ExceptionHelper.ThrowReadBufferExceeded();
        }
    }
}
