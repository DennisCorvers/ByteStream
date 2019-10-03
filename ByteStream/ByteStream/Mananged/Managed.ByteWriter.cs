using ByteStream.Interfaces;
using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Mananged
{
    /// <summary>
    /// A writer used to write values to a byte array.
    /// </summary>
    public class ByteWriter : ByteWriterLight
    {
        /// <summary>
        /// Creates a new instance of bytewriter with an empty buffer.
        /// </summary>
        public ByteWriter()
            : base() { }
        /// <summary>
        /// Creates a new instance of bytewriter with an empty byffer.
        /// </summary>
        /// <param name="initialSize">The initial size of the buffer.</param>
        /// <param name="isFixedSize">Determines if the buffer is allowed to increase its size automatically.</param>
        public ByteWriter(int initialSize, bool isFixedSize)
            : base(initialSize, isFixedSize) { }

        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        public ByteWriter(byte[] buffer)
            : base(buffer, true) { }
        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        /// <param name="isFixedSize">Determines if the buffer is allowed to increase its size automatically.</param>
        public ByteWriter(byte[] buffer, bool isFixedSize)
            : base(buffer, isFixedSize) { }

        /// <summary>
        /// Writes a boolean value.
        /// </summary>
        /// <param name="value"></param>
        public void WriteBool(bool value)
        {
            EnsureCapacity(sizeof(bool));
            BinaryHelper.WriteBool(m_buffer, m_offset, value);
            m_offset += sizeof(bool);
        }
        /// <summary>
        /// Writes a 8-bit signed integer.
        /// </summary>
        /// <param name="value"></param>
        public void WriteSByte(sbyte value)
        {
            EnsureCapacity(sizeof(sbyte));
            BinaryHelper.WriteSByte(m_buffer, m_offset, value);
            m_offset += sizeof(sbyte);
        }
        /// <summary>
        /// Writes a 8-bit unsigned integer.
        /// </summary>
        /// <param name="value"></param>
        public void WriteByte(byte value)
        {
            EnsureCapacity(sizeof(byte));
            BinaryHelper.WriteByte(m_buffer, m_offset, value);
            m_offset += sizeof(byte);
        }
        /// <summary>
        /// Writes a char in 2 bytes.
        /// </summary>
        /// <param name="value"></param>
        public void WriteChar(char value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a char in a single byte. (Default is 2)
        /// </summary>
        /// <param name="value"></param>
        public void WriteCharSingle(char value)
        {
            EnsureCapacity(sizeof(byte));
            BinaryHelper.WriteByte(m_buffer, m_offset, (byte)value);
            m_offset += sizeof(byte);
        }
        /// <summary>
        /// Writes a decimal as 4 32-bit integers.
        /// </summary>
        /// <param name="value"></param>
        public void WriteDecimal(decimal value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a double.
        /// </summary>
        /// <param name="value"></param>
        public void WriteDouble(double value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a floating precision point.
        /// </summary>
        /// <param name="value"></param>
        public void WriteSingle(float value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a 32-bit signed integer.
        /// </summary>
        /// <param name="value"></param>
        public void WriteInt(int value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a 32-bit unsigned integer.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUInt(uint value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a 64-bit signed integer.
        /// </summary>
        /// <param name="value"></param>
        public void WriteLong(long value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a 64-bit unsigned integer.
        /// </summary>
        /// <param name="value"></param>
        public void WriteULong(ulong value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a 16-bit signed integer.
        /// </summary>
        /// <param name="value"></param>
        public void WriteShort(short value)
        {
            WriteValueInternal(value);
        }
        /// <summary>
        /// Writes a 16-bit unsigned integer.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUShort(ushort value)
        {
            WriteValueInternal(value);
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
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="value"></param>
        public void WriteDBCS(string value)
        {
            EnsureCapacity(value.Length * sizeof(char));
            BinaryHelper.WriteDBCS(m_buffer, m_offset, value);
            m_offset += value.Length * sizeof(char);
        }
        /// <summary>
        /// Writes a string in ASCII encoding. Each character requires 1 byte.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="value"></param>
        public void WriteASCII(string value)
        {
            EnsureCapacity(value.Length);
            BinaryHelper.WriteASCII(m_buffer, m_offset, value);
            m_offset += value.Length;
        }
        /// <summary>
        /// Writes a string as a double-byte character set. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteDBCSLength(string value)
        {
            Write((ushort)value.Length);
            WriteDBCS(value);
        }
        /// <summary>
        /// Writes a string in ASCII encoding. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteASCIILength(string value)
        {
            Write((ushort)value.Length);
            WriteASCII(value);
        }
        /// <summary>
        /// Writes a string in UTF8 encoding. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUTF8(string value)
        {
            int byteSize = Encoding.UTF8.GetByteCount(value);

            EnsureCapacity(byteSize + sizeof(ushort));
            BinaryHelper.WriteValue(m_buffer, m_offset, (ushort)value.Length);
            m_offset += sizeof(ushort);

            BinaryHelper.WriteUTF8(m_buffer, m_offset, value);
            m_offset += byteSize;
        }
        /// <summary>
        /// Writes a string in UTF16 encoding. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteUTF16(string value)
        {
            int byteSize = Encoding.Unicode.GetByteCount(value);

            EnsureCapacity(byteSize + sizeof(ushort));
            BinaryHelper.WriteValue(m_buffer, m_offset, (ushort)value.Length);
            m_offset += sizeof(ushort);

            BinaryHelper.WriteUTF16(m_buffer, m_offset, value);
            m_offset += byteSize;
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
