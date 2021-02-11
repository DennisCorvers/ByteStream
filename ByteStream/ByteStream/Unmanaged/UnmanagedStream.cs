using ByteStream.Interfaces;
using ByteStream.Utils.Unsafe;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ByteStream.Unmanaged
{
    public unsafe class UnmanagedStream : IWriter, IReader, IByteStream
    {
        private const string BUFFER_OVERFLOW = "Buffer is set to a fixed size and cannot resize automatically.";

#pragma warning disable IDE0032
        private IntPtr m_buffer;
        private int m_offset;
        private int m_length;
        private SerializationMode m_mode;
#pragma warning restore

        /// <summary>
        /// The current <see cref="UnmanagedStream"/> offset in.
        /// </summary>
        public int Offset
            => m_offset;
        /// <summary>
        /// The total <see cref="UnmanagedStream"/> length in bytes.
        /// </summary>
        public int Length
            => m_length;

        /// <summary>
        /// The current streaming mode.
        /// </summary>
        public SerializationMode Mode
            => m_mode;
        /// <summary>
        /// Determines if the <see cref="UnmanagedStream"/> is writing.
        /// </summary>
        public bool IsWriting
            => m_mode == SerializationMode.Writing;
        /// <summary>
        /// Determines if the <see cref="UnmanagedStream"/> is reading.
        /// </summary>
        public bool IsReading
            => m_mode == SerializationMode.Reading;
        /// <summary>
        /// The inner buffer used by the <see cref="UnmanagedStream"/>.
        /// </summary>
        public IntPtr Buffer
            => m_buffer;


        public UnmanagedStream() { }


        /// <summary>
        /// Resets the <see cref="UnmanagedStream"/> for reading.
        /// </summary>
        public void ResetRead()
        {
            if (m_buffer == IntPtr.Zero)
                throw new InvalidOperationException("Stream has no buffer assigned.");

            Reset(0, SerializationMode.Reading);
        }

        /// <summary>
        /// Resets the <see cref="UnmanagedStream"/> for reading.
        /// </summary>
        /// <param name="data">The buffer to read from.</param>
        public void ResetRead(IntPtr data, int length)
        {
            m_buffer = data;
            m_length = length;

            ResetRead(data, 0, length);
        }

        /// <summary>
        /// Resets the <see cref="UnmanagedStream"/> for reading.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <param name="offset">The read offset.</param>
        /// <param name="length">The total amount of bytes available for reading.</param>
        public void ResetRead(IntPtr data, int offset, int length)
        {
            if (data == IntPtr.Zero)
                throw new ArgumentNullException(nameof(data));

            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            if ((uint)offset > length)
                throw new ArgumentOutOfRangeException("Offset must be smaller than length.");

            m_buffer = data;
            m_length = length;

            Reset(offset, SerializationMode.Reading);
        }

        /// <summary>
        /// Resets the <see cref="UnmanagedStream"/> for writing.
        /// </summary>
        /// <param name="data">The byte array to wrap.</param>
        /// <param name="length">The total amount of bytes available for writing.</param>
        public void ResetWrite(IntPtr data, int length)
        {
            if (data == IntPtr.Zero)
                throw new ArgumentNullException(nameof(data));

            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            m_buffer = data;
            m_length = length;

            Reset(0, SerializationMode.Writing);
        }

        /// <summary>
        /// Resets the <see cref="UnmanagedStream"/> for writing.
        /// </summary>
        /// <param name="data">The byte array to wrap.</param>
        /// <param name="offset">The write offset.</param>
        public void ResetWrite(IntPtr data, int offset, int length)
        {
            if (data == IntPtr.Zero)
                throw new ArgumentNullException(nameof(data));

            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            if ((uint)offset > length)
                throw new ArgumentOutOfRangeException("Offset must be smaller than length.");

            m_buffer = data;
            m_length = length;

            Reset(offset, SerializationMode.Writing);
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

            EnsureCapacity(amount);
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
        /// Reads a blittable struct or primitive value from the <see cref="UnmanagedStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnmanagedStream Peek<T>(ref T value) where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);
            value = BinaryHelper.Read<T>(m_buffer, m_offset);

            return this;
        }

        /// <summary>
        /// Reads a blittable struct or primitive value from the <see cref="UnmanagedStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnmanagedStream Read<T>(ref T value) where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);

            value = BinaryHelper.Read<T>(m_buffer, m_offset);
            m_offset += size;

            return this;
        }

        /// <summary>
        /// Reads a value from the <see cref="UnmanagedStream"/>. Can overwrite value with default.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>() where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);

            var value = BinaryHelper.Read<T>(m_buffer, m_offset);
            m_offset += size;

            return value;
        }

        /// <summary>
        /// Writes a blittable struct or primitive value to the <see cref="UnmanagedStream"/>.
        /// </summary>
        /// <typeparam name="T">The type of the blittable struct/primitive.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnmanagedStream Write<T>(T value) where T : unmanaged
        {
            int size = sizeof(T);
            EnsureCapacity(size);
            BinaryHelper.Write(m_buffer, m_offset, value);
            m_offset += size;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnmanagedStream Serialize<T>(ref T value) where T : unmanaged
        {
            if (m_mode == SerializationMode.Writing) Write(value);
            else Read(ref value);

            return this;
        }


        /// <summary>
        /// Writes a byte array to the <see cref="UnmanagedStream"/>.
        /// </summary>
        /// <param name="includeSize">TRUE to include the size as an uint16</param>
        public UnmanagedStream WriteBytes(byte[] value, bool includeSize = true)
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

            return this;
        }

        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="includeSize">TRUE to include the size as an uint16</param>
        public UnmanagedStream WriteUTF16(string value, bool includeSize = true)
        {
            if (includeSize)
            {
                if (value.Length > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), "Maximum size of 65.535 exceeded.");

                Write((ushort)value.Length);
            }

            EnsureCapacity(value.Length * sizeof(char));
            StringHelper.WriteANSI(m_buffer, m_offset, value);
            m_offset += value.Length * sizeof(char);

            return this;
        }

        /// <summary>
        /// Writes a string in ANSI encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="includeSize">TRUE to include the size as an uint16</param>
        public UnmanagedStream WriteANSI(string value, bool includeSize = true)
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

            return this;
        }

        /// <summary>
        /// Writes a string to the <see cref="ByteWriter"/>.
        /// Includes the bytesize as a uint16.
        /// </summary>
        public UnmanagedStream WriteString(string value, Encoding encoding)
        {
            int byteCount = encoding.GetByteCount(value);

            if (byteCount > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value), "String is too large to be written.");

            EnsureCapacity(byteCount + sizeof(ushort));

            Write((ushort)byteCount);
            StringHelper.WriteString(m_buffer, m_offset, m_length, value, encoding);

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
            EnsureCapacity(length);

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
            EnsureCapacity(stringLength * sizeof(char));

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
            EnsureCapacity(stringLength);
            var value = StringHelper.ReadANSI(m_buffer, m_offset, stringLength);

            m_offset += stringLength;

            return value;
        }

        /// <summary>
        /// Reads a string from the <see cref="UnmanagedStream"/>.
        /// Length is automatically retrieved.
        /// </summary>
        /// <returns></returns>
        public string ReadString(Encoding encoding)
        {
            ushort byteCount = 0;
            Read(ref byteCount);

            EnsureCapacity(byteCount);
            var value = StringHelper.ReadString((IntPtr)m_buffer, m_offset, byteCount, encoding);
            m_offset += byteCount;

            return value;
        }

        public void SerializeString(ref string value, Encoding encoding)
        {
            if (m_mode == SerializationMode.Reading)
                value = ReadString(encoding);
            else
                WriteString(value, encoding);
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
                throw new ArgumentNullException(nameof(ptr));

            if (destinationIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));

            if ((uint)length > Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            Memory.CopyMemory((void*)m_buffer, (byte*)ptr + destinationIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int size)
        {
            if (m_offset + size > m_length)
                throw new InvalidOperationException(BUFFER_OVERFLOW);
        }

        /// <summary>
        /// Tries to write a value to the <see cref="UnmanagedStream"/>. Returns FALSE if the value couldn't be written.
        /// </summary>
        public bool TryWrite<T>(T value) where T : unmanaged
        {
            int size = sizeof(T);
            if (m_offset + size > m_length)
            {
                return false;
            }

            BinaryHelper.Write(m_buffer, m_offset, value);
            m_offset += size;

            return true;
        }

        /// <summary>
        /// Tries to read a value from the <see cref="UnmanagedStream"/>. Returns FALSE if the value couldn't be read.
        /// </summary>
        public bool TryRead<T>(out T value) where T : unmanaged
        {
            int size = sizeof(T);
            if (m_offset + size > m_length)
            {
                value = default;
                return false;
            }

            value = BinaryHelper.Read<T>(m_buffer, m_offset);
            m_offset += size;

            return true;
        }

        void IBuffer.Clear()
        {
            Reset(0, SerializationMode.None);
        }

        void IWriter.Write<T>(T value)
        {
            Write(value);
        }

        void IByteStream.Serialize<T>(ref T value)
        {
            Serialize(ref value);
        }
    }
}
