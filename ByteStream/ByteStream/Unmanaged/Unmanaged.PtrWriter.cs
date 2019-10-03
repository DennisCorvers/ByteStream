using ByteStream.Interfaces;
using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Unmanaged
{
    public class PtrWriter : IWriter
    {
        public delegate IntPtr ReAlloc(IntPtr ptr, int newSize);

        private const int DEFAULTSIZE = 64;

#pragma warning disable IDE0032
        private int m_length;
        private ReAlloc m_realloc;

        protected IntPtr m_buffer;
        protected int m_offset;
        protected bool m_isFixedSize;
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
        /// Determines if this writer has a fixed size.
        /// </summary>
        public bool IsFixedSize
            => m_isFixedSize;
        /// <summary>
        /// Gets the internal buffer used by this writer.
        /// Do not modify the buffer while performing write operations.
        /// </summary>
        public IntPtr Buffer
            => m_buffer;

        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        /// <param name="bufferLength">The amount of bytes that can be written.</param>
        public PtrWriter(IntPtr buffer, int bufferLength)
            : this(buffer, bufferLength, true)
        { }
        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        /// <param name="bufferLength">The amount of bytes that can be written.</param>
        /// <param name="isFixedSize">Determines if the buffer is allowed to increase its size automatically.</param>
        public PtrWriter(IntPtr buffer, int bufferLength, bool isFixedSize)
            : this(buffer, bufferLength, isFixedSize, Memory.ResizePtr)
        { }
        /// <summary>
        /// Creates a new instance of bytewriter from an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use with this writer.</param>
        /// <param name="bufferLength">The amount of bytes that can be written.</param>
        /// <param name="isFixedSize">Determines if the buffer is allowed to increase its size automatically.</param>
        /// <param name="reAllocCallback">A callback that is used for reallocating the buffer.</param>
        public PtrWriter(IntPtr buffer, int bufferLength, bool isFixedSize, ReAlloc reAllocCallback)
        {
            if (buffer == IntPtr.Zero) { throw new ArgumentNullException("buffer"); }

            m_buffer = buffer;
            m_isFixedSize = isFixedSize;
            m_offset = 0;
            m_length = bufferLength;
            m_realloc = reAllocCallback;
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
        /// Performs a memory copy.
        /// </summary>
        /// <param name="newSize">The new size of the buffer.</param>
        public void Resize(int newSize)
        {
            if (newSize < 0)
            { throw new ArgumentOutOfRangeException("newSize"); }
            if (newSize < m_offset)
            { throw new InvalidOperationException("New size is smaller than the current write offset!"); }

            m_buffer = m_realloc(m_buffer, newSize);
        }
        /// <summary>
        /// Resizes the current buffer to the current write offset.
        /// Performs a memory copy.
        /// </summary>
        public void Trim()
        {
            m_buffer = m_realloc(m_buffer, m_offset);
        }
        /// <summary>
        /// Resets the offset to zero.
        /// </summary>
        public void Clear()
        {
            m_offset = 0;
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
        public void WriteString(string value)
        {
            EnsureCapacity(value.Length * sizeof(char));
            StringHelper.WriteString(m_buffer, m_offset, value);
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
            StringHelper.WriteASCII(m_buffer, m_offset, value);
            m_offset += value.Length;
        }
        /// <summary>
        /// Writes a string as a double-byte character set. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="value"></param>
        public void WriteStringLength(string value)
        {
            Write((ushort)value.Length);
            WriteString(value);
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
            BinaryHelper.Write(m_buffer, m_offset, (ushort)value.Length);
            m_offset += sizeof(ushort);

            StringHelper.WriteUTF8(m_buffer, m_offset, value, byteSize);
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
            BinaryHelper.Write(m_buffer, m_offset, (ushort)value.Length);
            m_offset += sizeof(ushort);

            StringHelper.WriteUTF16(m_buffer, m_offset, value, byteSize);
            m_offset += byteSize;
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
        { CopyToBytes(buffer, 0, m_offset); }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyToBytes(byte[] buffer, int destinationIndex)
        { CopyToBytes(buffer, destinationIndex, m_offset); }
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

            Memory.CopyMemory(m_buffer, 0, buffer, destinationIndex, length);
        }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyToPtr(IntPtr ptr)
        { CopyToPtr(ptr, 0, m_offset); }
        /// <summary>
        /// Copies the inner buffer to a supplied buffer.
        /// </summary>
        /// <param name="buffer">The destination for the data.</param>
        public void CopyToPtr(IntPtr ptr, int destinationIndex)
        { CopyToPtr(ptr, destinationIndex, m_offset); }
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

            m_buffer = m_realloc(m_buffer, newSize);
        }
    }
}
