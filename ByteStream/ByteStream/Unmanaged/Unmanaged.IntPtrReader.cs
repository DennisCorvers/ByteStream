using ByteStream.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace ByteStream.Unmanaged
{
    public class IntPtrReader : IReader
    {
#pragma warning disable IDE0032
        private int m_length;

        protected IntPtr m_buffer;
        protected int m_offset;
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
        /// <param name="buffer">The data to be read by this bytereader.</param>
        /// <param name="bufferLength">The amount of bytes that can be read.</param>
        public IntPtrReader(IntPtr buffer, int bufferLength)
        {
            if (buffer == IntPtr.Zero) { throw new ArgumentNullException("buffer"); }
            if (bufferLength < 1) { throw new ArgumentOutOfRangeException("bufferLength"); }

            m_offset = 0;
            m_length = bufferLength;
            m_buffer = buffer;
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
        /// Reads a boolean from the current packet.
        /// </summary>
        public bool ReadBool()
        {
            EnsureCapacity(sizeof(bool));
            bool val = BinaryHelper.ReadBool(m_buffer, m_offset);

            m_offset += sizeof(bool);
            return val;
        }
        /// <summary>
        /// Reads a 8-bit unsigned integer from the current packet.
        /// </summary>
        public byte ReadByte()
        {
            EnsureCapacity(sizeof(byte));
            byte val = BinaryHelper.ReadByte(m_buffer, m_offset);

            m_offset += sizeof(byte);
            return val;
        }
        /// <summary>
        /// Reads a 8-bit signed integer from the current packet.
        /// </summary>
        public sbyte ReadSByte()
        {
            EnsureCapacity(sizeof(sbyte));
            sbyte val = BinaryHelper.ReadSByte(m_buffer, m_offset);

            m_offset += sizeof(sbyte);
            return val;
        }
        /// <summary>
        /// Reads a character from the current packet. Uses 2 bytes.
        /// </summary>
        public char ReadChar()
        {
            return ReadValueInternal<char>();
        }
        /// <summary>
        /// Reads a character from the current packet. Uses 1 byte (default is 2).
        /// </summary>
        /// <returns></returns>
        public char ReadCharSingle()
        {
            EnsureCapacity(sizeof(byte));
            char val = BinaryHelper.ReadCharSingle(m_buffer, m_offset);

            m_offset += sizeof(byte);
            return val;
        }
        /// <summary>
        /// Reads a decimal from the current packet as 4 32-bit unsigned integers.
        /// </summary>
        public decimal ReadDecimal()
        {
            return ReadValueInternal<decimal>();
        }
        /// <summary>
        /// Reads a double from the current packet.
        /// </summary>
        public double ReadDouble()
        {
            return ReadValueInternal<double>();
        }
        /// <summary>
        /// Reads a floating precision point from the current packet.
        /// </summary>
        public float ReadSingle()
        {
            return ReadValueInternal<float>();
        }
        /// <summary>
        /// Reads a 32-bit signed integer from the current packet.
        /// </summary>
        public int ReadInt()
        {
            return ReadValueInternal<int>();
        }
        /// <summary>
        /// Reads a 32-bit unsigned integer from the current packet.
        /// </summary>
        public uint ReadUInt()
        {
            return ReadValueInternal<uint>();
        }
        /// <summary>
        /// Reads a 64-bit signed integer from the current packet.
        /// </summary>
        public long ReadLong()
        {
            return ReadValueInternal<long>();
        }
        /// <summary>
        /// Reads a 64-bit unsigned integer from the current packet.
        /// </summary>
        public ulong ReadULong()
        {
            return ReadValueInternal<ulong>();
        }
        /// <summary>
        /// Reads a 16-bit signed integer from the current packet.
        /// </summary>
        public short ReadShort()
        {
            return ReadValueInternal<short>();
        }
        /// <summary>
        /// Reads a 16-bit unsigned integer from the current packet.
        /// </summary>
        public ushort ReadUShort()
        {
            return ReadValueInternal<ushort>();
        }
        /// <summary>
        /// Reads a byte-array from the current packet. Length is automatically read as an uint16.
        /// </summary>
        public byte[] ReadBytesLength()
        {
            ushort length = ReadValue<ushort>();
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
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// Does NOT automatically read length.
        /// </summary>
        public string ReadDBCS(int stringLength)
        {
            EnsureCapacity(stringLength * sizeof(char));
            string val = BinaryHelper.ReadDBCS(m_buffer, m_offset, stringLength);

            m_offset += stringLength * sizeof(char);
            return val;
        }
        /// <summary>
        /// Reads a string in ASCII encoding. Each character requires 1 byte.
        /// Does NOT automatically read length.
        /// </summary>
        public string ReadASCII(int stringLength)
        {
            EnsureCapacity(stringLength);
            string val = BinaryHelper.ReadASCII(m_buffer, m_offset, stringLength);

            m_offset += stringLength;
            return val;
        }
        /// <summary>
        /// Reads a string as a double-byte character set. Length is automatically retrieved as an uint16.
        /// </summary>
        public string ReadDBCSLength()
        {
            ushort lengh = ReadValue<ushort>();
            return ReadDBCS(Length);
        }
        /// <summary>
        /// Reads a string in ASCII encoding. Length is automatically retrieved as an uint16.
        /// </summary>
        public string ReadASCIILength()
        {
            ushort length = ReadValue<ushort>();
            return ReadASCII(length);
        }
        /// <summary>
        /// Reads a string in UTF8 encoding. Length is automatically retrieved as an uint16.
        /// </summary>
        public string ReadUTF8()
        {
            ushort length = ReadValue<ushort>();
            EnsureCapacity(length);
            string val = BinaryHelper.ReadUTF8(m_buffer, m_offset, length);

            m_offset += length;
            return val;

        }
        /// <summary>
        /// Reads a string in UTF16 encoding. Length is automatically retrieved as an uint16.
        /// </summary>
        public string ReadUTF16()
        {
            ushort length = ReadValue<ushort>();
            EnsureCapacity(length);
            string val = BinaryHelper.ReadUTF16(m_buffer, m_offset, length);

            m_offset += length;
            return val;
        }

        /// <summary>
        /// Reades a blittable struct or primitive value from the buffer.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        public T ReadValue<T>() where T : unmanaged
        {
            return ReadValueInternal<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe T ReadValueInternal<T>() where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);
            T val = BinaryHelper.ReadValue<T>(m_buffer, m_offset);

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
