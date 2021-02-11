using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStream.Mananged
{
    public unsafe class ManagedStream
    {
        public const int DefaultBufferSize = 1024;
        private const string BUFFER_OVERFLOW = "Buffer is set to a fixed size and cannot resize automatically.";

#pragma warning disable IDE0032
        private byte[] m_buffer;
        private int m_offset;
        private SerializationMode m_mode;

        private bool m_isFixedSize;
#pragma warning restore

        /// <summary>
        /// The current <see cref="ManagedStream"/> offset in.
        /// </summary>
        public int Offset
            => m_offset;
        /// <summary>
        /// The total <see cref="ManagedStream"/> length in bytes.
        /// </summary>
        public int Length
            => m_buffer.Length;

        /// <summary>
        /// The current streaming mode.
        /// </summary>
        public SerializationMode Mode
            => m_mode;
        /// <summary>
        /// Determines if the <see cref="ManagedStream"/> is writing.
        /// </summary>
        public bool IsWriting
            => m_mode == SerializationMode.Writing;
        /// <summary>
        /// Determines if the <see cref="ManagedStream"/> is reading.
        /// </summary>
        public bool IsReading
            => m_mode == SerializationMode.Reading;
        /// <summary>
        /// The inner buffer used by the <see cref="ManagedStream"/>.
        /// </summary>
        public byte[] Buffer
            => m_buffer;
        /// <summary>
        /// A fixed-size buffer will not resize itself.
        /// </summary>
        public bool IsFixedSize
            => m_isFixedSize;


        public ManagedStream() { }


        /// <summary>
        /// Resets the <see cref="ManagedStream"/> for reading.
        /// </summary>
        public void ResetRead()
        {
            if (m_buffer == null)
                throw new InvalidOperationException("Stream has no buffer assigned.");

            Reset(0, SerializationMode.Reading);
        }

        /// <summary>
        /// Resets the <see cref="ManagedStream"/> for reading.
        /// </summary>
        /// <param name="data">The buffer to read from.</param>
        public void ResetRead(byte[] data)
        {
            m_buffer = data;
            Reset(0, SerializationMode.Reading);
        }

        /// <summary>
        /// Resets the <see cref="ManagedStream"/> for reading.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <param name="offset">The read offset.</param>
        /// <param name="length">The total amount of bytes available for reading.</param>
        public void ResetRead(byte[] data, int offset, int length)
        {
            m_buffer = data ?? throw new ArgumentNullException(nameof(data));

            if ((uint)(offset + length) > data.Length)
                throw new ArgumentOutOfRangeException("Offset + Length must be smaller than array length.");

            Reset(offset, SerializationMode.Reading);
        }


        /// <summary>
        /// Resets the <see cref="ManagedStream"/> for writing.
        /// </summary>
        public void ResetWrite()
        {
            m_buffer = new byte[DefaultBufferSize];
            m_isFixedSize = false;
            Reset(0, SerializationMode.Writing);
        }

        /// <summary>
        /// Resets the <see cref="ManagedStream"/> for writing.
        /// </summary>
        /// <param name="initialSize">The starting size of the buffer.</param>
        /// <param name="isFixedSize">FALSE if the buffer can automatically resize.</param>
        public void ResetWrite(int initialSize, bool isFixedSize = false)
        {
            if (initialSize < 1)
                throw new ArgumentException(nameof(initialSize));

            m_buffer = new byte[initialSize];
            m_isFixedSize = isFixedSize;

            Reset(0, SerializationMode.Writing);
        }

        /// <summary>
        /// Resets the <see cref="ManagedStream"/> for writing.
        /// </summary>
        /// <param name="data">The byte array to wrap.</param>
        /// <param name="isFixedSize">FALSE if the buffer can automatically resize.</param>
        public void ResetWrite(byte[] data, bool isFixedSize = false)
        {
            m_buffer = data ?? throw new ArgumentNullException(nameof(data));
            m_isFixedSize = isFixedSize;

            Reset(0, SerializationMode.Writing);
        }

        /// <summary>
        /// Resets the <see cref="ManagedStream"/> for writing.
        /// </summary>
        /// <param name="data">The byte array to wrap.</param>
        /// <param name="offset">The write offset.</param>
        public void ResetWrite(byte[] data, int offset)
        {
            m_buffer = data ?? throw new ArgumentNullException(nameof(data));
            m_isFixedSize = true;

            Reset(0, SerializationMode.Writing);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Reset(int offset, SerializationMode mode)
        {
            m_offset = offset;
            m_mode = mode;
        }


        /// <summary>
        /// Increases the write offset by some amount.
        /// </summary>
        /// <param name="amount">The amounts of bytes to skip.</param>
        public void SkipBytes(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount needs to be at least 1.");

            EnsureWriteCapacity(amount);
            m_offset += amount;
        }

        /// <summary>
        /// Reserves 4-bytes of space for a size value at the start of the <see cref="ByteWriter"/>.
        /// </summary>
        public void ReserveSizePrefix()
        {
            SkipBytes(sizeof(int));
        }

        /// <summary>
        /// Writes the total size at the start of the <see cref="ByteWriter"/> as an <see cref="int"/>.
        /// </summary>
        public int PrefixSize()
        {
            BinaryHelper.Write(m_buffer, 0, m_offset);
            return m_offset;
        }


        /// <summary>
        /// Reads a blittable struct or primitive value from the <see cref="ManagedStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ManagedStream Peek<T>(ref T value) where T : unmanaged
        {
            int size = sizeof(T);
            EnsureReadCapacity(size);
            value = BinaryHelper.Read<T>(m_buffer, m_offset);

            return this;
        }

        /// <summary>
        /// Reads a blittable struct or primitive value from the <see cref="ManagedStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ManagedStream Read<T>(ref T value) where T : unmanaged
        {
            int size = sizeof(T);
            EnsureReadCapacity(size);

            value = BinaryHelper.Read<T>(m_buffer, m_offset);
            m_offset += size;

            return this;
        }

        /// <summary>
        /// Reads a value from the <see cref="ManagedStream"/>. Can overwrite value with default.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>() where T : unmanaged
        {
            int size = sizeof(T);
            EnsureReadCapacity(size);

            var value = BinaryHelper.Read<T>(m_buffer, m_offset);
            m_offset += size;

            return value;
        }

        /// <summary>
        /// Writes a blittable struct or primitive value to the <see cref="ManagedStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ManagedStream Write<T>(T value) where T : unmanaged
        {
            int size = sizeof(T);
            EnsureWriteCapacity(size);
            BinaryHelper.Write(m_buffer, m_offset, value);
            m_offset += size;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ManagedStream Serialize<T>(ref T value) where T : unmanaged
        {
            if (m_mode == SerializationMode.Writing) Write(value);
            else Read(ref value);

            return this;
        }


        /// <summary>
        /// Writes a byte array to the <see cref="ManagedStream"/>.
        /// </summary>
        /// <param name="includeSize">TRUE to include the size as an uint16</param>
        public ManagedStream WriteBytes(byte[] value, bool includeSize = true)
        {
            if (includeSize)
            {
                if (value.Length > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum size of 65.535 exceeded.");

                Write((ushort)value.Length);
            }

            EnsureReadCapacity(value.Length);
            BinaryHelper.WriteBytes(m_buffer, m_offset, value);
            m_offset += value.Length;

            return this;
        }

        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="includeSize">TRUE to include the size as an uint16</param>
        public ManagedStream WriteUTF16(string value, bool includeSize = true)
        {
            if (includeSize)
            {
                if (value.Length > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum size of 65.535 exceeded.");

                Write((ushort)value.Length);
            }

            EnsureWriteCapacity(value.Length * sizeof(char));
            StringHelper.WriteANSI(m_buffer, m_offset, value);
            m_offset += value.Length * sizeof(char);

            return this;
        }

        /// <summary>
        /// Writes a string in ANSI encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="includeSize">TRUE to include the size as an uint16</param>
        public ManagedStream WriteANSI(string value, bool includeSize = true)
        {
            if (includeSize)
            {
                if (value.Length > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum size of 65.535 exceeded.");

                Write((ushort)value.Length);
            }

            EnsureWriteCapacity(value.Length);
            StringHelper.WriteANSI(m_buffer, m_offset, value);
            m_offset += value.Length;

            return this;
        }

        /// <summary>
        /// Writes a string to the <see cref="ByteWriter"/>.
        /// Includes the bytesize as a uint16.
        /// </summary>
        public ManagedStream WriteString(string value, Encoding encoding)
        {
            int byteCount = encoding.GetByteCount(value);

            if (byteCount > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value), "String is too large to be written.");

            EnsureWriteCapacity(byteCount + sizeof(ushort));

            Write((ushort)byteCount);
            StringHelper.WriteString(m_buffer, m_offset, value, encoding);

            m_offset += byteCount;

            return this;
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
            EnsureReadCapacity(length);

            var value = BinaryHelper.ReadBytes(m_buffer, m_offset, length);

            m_offset += length;

            return value;
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
            EnsureReadCapacity(stringLength * sizeof(char));

            var value = StringHelper.ReadUTF16(m_buffer, m_offset, stringLength);

            m_offset += stringLength * sizeof(char);

            return value;
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
            EnsureReadCapacity(stringLength);
            var value = StringHelper.ReadANSI(m_buffer, m_offset, stringLength);

            m_offset += stringLength;

            return value;
        }

        /// <summary>
        /// Reads a string from the <see cref="ByteReader"/>.
        /// Length is automatically retrieved.
        /// </summary>
        /// <returns></returns>
        public string ReadString(Encoding encoding)
        {
            ushort byteCount = 0;
            Read(ref byteCount);

            EnsureReadCapacity(byteCount);
            var value = StringHelper.ReadString(m_buffer, m_offset, byteCount, encoding);
            m_offset += byteCount;

            return value;
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

            m_buffer.CopyToUnsafe(0, buffer, destinationIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureWriteCapacity(int size)
        {
            if (m_offset + size > m_buffer.Length)
            {
                if (m_isFixedSize)
                {
                    throw new InvalidOperationException(BUFFER_OVERFLOW);
                }

                ArrayExtensions.ResizeUnsafe(ref m_buffer, MathUtils.NextPowerOfTwo(m_offset + size));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureReadCapacity(int size)
        {
            if (m_offset + size > m_buffer.Length)
                throw new InvalidOperationException(BUFFER_OVERFLOW);
        }
    }
}
