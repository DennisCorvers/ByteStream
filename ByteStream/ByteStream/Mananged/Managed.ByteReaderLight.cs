using ByteStream.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Mananged
{
    public class ByteReaderLight : IReader
    {
#pragma warning disable IDE0032
        protected byte[] m_buffer;
        protected int m_offset;
#pragma warning restore IDE0032

        /// <summary>
        /// The length of this reader.
        /// </summary>
        public int Length
            => m_buffer.Length;
        /// <summary>
        /// The current read offset.
        /// </summary>
        public int Offset
            => m_offset;
        /// <summary>
        /// Determines if this reader has a fixed size.
        /// </summary>
        public virtual bool IsFixedSize
            => true;

        /// <summary>
        /// Creates a new instance of bytereader.
        /// </summary>
        /// <param name="data">The data to be read by this bytereader.</param>
        public ByteReaderLight(byte[] data)
        {
            m_buffer = data ?? throw new ArgumentNullException("data");
            m_offset = 0;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal unsafe T ReadValueInternal<T>() where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);
            T val = BinaryHelper.ReadValue<T>(m_buffer, m_offset);

            m_offset += size;
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureCapacity(int bytesToRead)
        {
            if (bytesToRead + m_offset > m_buffer.Length)
            { throw new InvalidOperationException("Read operation exceeds buffer size!"); }
        }
    }
}
