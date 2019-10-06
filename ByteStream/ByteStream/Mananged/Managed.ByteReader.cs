using ByteStream.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Mananged
{
    public struct ByteReader : IReader
    {
#pragma warning disable IDE0032
        private byte[] m_buffer;
        private int m_offset;
        private int m_length;
#pragma warning restore IDE0032

        /// <summary>
        /// The length of this reader.
        /// </summary>
        public int Length
            => m_length;
        /// <summary>
        /// The current read offset.
        /// </summary>
        public int Offset
            => m_offset;
        /// <summary>
        /// Determines if this reader has a fixed size.
        /// </summary>
        public bool IsFixedSize
            => true;

        /// <summary>
        /// Creates a new instance of bytereader.
        /// </summary>
        /// <param name="data">The data to read.</param>
        public ByteReader(byte[] data)
            : this(data, 0, data.Length)
        { }

        /// <summary>
        /// Creates a new instance of bytereader.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <param name="offset">The read offset.</param>
        /// <param name="length">The total amount of bytes available for reading.</param>
        public ByteReader(byte[] data, int offset, int length)
        {
            m_buffer = data ?? throw new ArgumentNullException("data");

            if (offset < 0)
            { throw new ArgumentOutOfRangeException("offset"); }
            if (length < 0)
            { throw new ArgumentOutOfRangeException("count"); }
            if (data.Length - offset < length)
            { throw new ArgumentException("Array out of bounds."); }

            m_offset = offset;
            m_length = length;
        }

        /// <summary>
        /// Increases the read offset by some amount.
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
        /// Reades a blittable struct or primitive value from the buffer.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        public T Read<T>() where T : unmanaged
        {
            return ReadValueInternal<T>();
        }

        /// <summary>
        /// Reads a byte-array from the current packet. Length is automatically read as an uint16.
        /// </summary>
        public byte[] ReadBytes()
        {
            ushort length = Read<ushort>();
            return ReadBytes(length);
        }
        /// <summary>
        /// Reads a byte-array from the current packet.
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
            ushort lengh = Read<ushort>();
            return ReadUTF16(Length);
        }
        /// <summary>
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// Does NOT automatically read length.
        /// </summary>
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
        /// Reads a string in UTF8 encoding. Length is automatically retrieved as an uint16.
        /// </summary>
        public string ReadUTF8()
        {
            ushort length = Read<ushort>();
            EnsureCapacity(length);
            string val = StringHelper.ReadUTF8(m_buffer, m_offset, length);

            m_offset += length;
            return val;
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
            { throw new InvalidOperationException("Read operation exceeds buffer size!"); }
        }
    }
}
