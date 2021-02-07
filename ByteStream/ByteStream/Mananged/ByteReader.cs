using ByteStream.Interfaces;
using System;
using System.Runtime.CompilerServices;

namespace ByteStream.Mananged
{
    public struct ByteReader : IReader
    {
#pragma warning disable IDE0032
        private readonly byte[] m_buffer;
        private readonly int m_length;
        private int m_offset;
#pragma warning restore IDE0032

        /// <summary>
        /// The length of this <see cref="ByteReader"/>.
        /// </summary>
        public int Length
            => m_length;
        /// <summary>
        /// The current read offset.
        /// </summary>
        public int Offset
            => m_offset;
        /// <summary>
        /// Determines if the <see cref="ByteReader"/> has a fixed size.
        /// </summary>
        public bool IsFixedSize
            => true;
        /// <summary>
        /// Gets the internal buffer used by this <see cref="ByteReader"/>.
        /// Do not modify the buffer while performing read operations.
        /// </summary>
        public byte[] Buffer
            => m_buffer;


        /// <summary>
        /// Creates a new instance of <see cref="ByteReader"/>.
        /// </summary>
        /// <param name="data">The data to read.</param>
        public ByteReader(byte[] data)
        {
            m_buffer = data ?? throw new ArgumentNullException("data");

            m_offset = 0;
            m_length = data.Length;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ByteReader"/>.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <param name="offset">The read offset.</param>
        /// <param name="length">The total amount of bytes available for reading.</param>
        public ByteReader(byte[] data, int offset, int length)
        {
            m_buffer = data ?? throw new ArgumentNullException(nameof(data));

            if ((uint)(offset + length) > data.Length)
                throw new ArgumentOutOfRangeException("Offset + Length must be smaller than array length.");

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
                throw new ArgumentOutOfRangeException(nameof(amount));

            EnsureCapacity(amount);
            m_offset += amount;
        }

        /// <summary>
        /// Resets the offset to its original position.
        /// </summary>
        public void Clear()
        {
            m_offset = 0;
        }

        /// <summary>
        /// Reads a blittable struct or primitive value from the <see cref="ByteReader"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        public T Read<T>() where T : unmanaged
        {
            unsafe
            {
                int size = sizeof(T);
                EnsureCapacity(size);
                T val = BinaryHelper.Read<T>(m_buffer, m_offset);

                m_offset += size;
                return val;
            }
        }
        /// <summary>
        /// Tries to read a blittable struct or primitive value from the <see cref="ByteReader"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        /// <returns>Returns false if the value couldn't be read.</returns>
        public bool TryRead<T>(out T value) where T : unmanaged
        {
            value = default;

            unsafe
            {
                int size = sizeof(T);
                if (m_offset + size > Length)
                    return false;

                value = BinaryHelper.Read<T>(m_buffer, m_offset);
                m_offset += size;
            }

            return true;
        }

        /// <summary>
        /// Reads a byte-array from the <see cref="ByteReader"/>. Length is automatically read as an uint16.
        /// </summary>
        public byte[] ReadBytes()
        {
            ushort length = Read<ushort>();
            return ReadBytes(length);
        }
        /// <summary>
        /// Reads a byte-array from the <see cref="ByteReader"/>.
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int bytesToRead)
        {
            if (bytesToRead + m_offset > m_length)
                throw new InvalidOperationException("Read operation exceeds buffer size.");
        }
    }
}
