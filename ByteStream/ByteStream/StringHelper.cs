using ByteStream.Utils.Unsafe;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStream
{
    public unsafe static class StringHelper
    {
        private const int StrBufSize = 1024;

        /// <summary>
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="data">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadUTF16(byte[] data, int offset, int stringLength)
        {
            if (stringLength < 1)
                return string.Empty;

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            fixed (byte* ptr = &data[offset])
            {
                return new string((char*)ptr, 0, stringLength);
            }
        }

        /// <summary>
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadUTF16(IntPtr data, int offset, int stringLength)
        {
            if (stringLength < 1)
                return string.Empty;

            if (data == IntPtr.Zero)
                throw new ArgumentNullException(nameof(data));

            return new string((char*)data, offset, stringLength);
        }


        /// <summary>
        /// Reads a string in ANSI encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadANSI(byte[] data, int offset, int stringLength)
        {
            if (stringLength < 1)
                return string.Empty;

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            fixed (byte* ptr = data)
            {
                return new string((sbyte*)ptr, offset, stringLength);
            }
        }

        /// <summary>
        /// Reads a string in ANSI encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadANSI(IntPtr data, int offset, int stringLength)
        {
            if (stringLength < 1)
                return string.Empty;

            if (data == IntPtr.Zero)
                throw new ArgumentNullException(nameof(data));

            return new string((sbyte*)data, offset, stringLength);
        }


        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static int WriteUTF16(byte[] dest, int offset, string value)
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            int length = value.Length * sizeof(char);

            fixed (byte* ptr = &dest[offset])
            fixed (char* str = value)
            {
                Buffer.MemoryCopy(str, ptr, length, length);
            }

            return length;
        }

        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static int WriteUTF16(IntPtr dest, int offset, string value)
        {
            if (dest == IntPtr.Zero)
                throw new ArgumentNullException(nameof(dest));

            for (int i = 0; i < value.Length; i++)
                *((char*)dest + offset + i) = value[i];

            return value.Length * sizeof(char);
        }


        /// <summary>
        /// Writes a string in ANSI encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static int WriteANSI(byte[] dest, int offset, string value)
        {
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

            for (int i = 0; i < value.Length; i++)
                dest[offset + i] = (byte)value[i];

            return value.Length;
        }

        /// <summary>
        /// Writes a string in ANSI encoding. Each character requires 1 byte.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static int WriteANSI(IntPtr dest, int offset, string value)
        {
            if (dest == IntPtr.Zero)
                throw new ArgumentNullException(nameof(dest));

            for (int i = 0; i < value.Length; i++)
                *((byte*)dest + offset + i) = (byte)value[i];

            return value.Length;
        }


        /// <summary>
        /// Writes a string to the supplied buffer.
        /// </summary>
        public static int WriteString(byte[] data, int offset, string value, Encoding encoding)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if ((uint)offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            return encoding.GetBytes(value, 0, value.Length, data, offset);
        }

        /// <summary>
        /// Writes a string to the supplied buffer.
        /// </summary>
        /// <param name="dataLength">The length of the supplied buffer.</param>
        public static int WriteString(IntPtr data, int offset, int dataLength, string value, Encoding encoding)
        {
            if (data == IntPtr.Zero)
                throw new ArgumentNullException(nameof(data));

            if ((uint)offset > dataLength)
                throw new ArgumentOutOfRangeException("Offset must be smaller than dataLength");

            fixed (char* str = value)
            {
                return encoding.GetBytes(str, value.Length, ((byte*)data) + offset, dataLength);
            }
        }

        /// <summary>
        /// Reads a string from the supplied buffer.
        /// </summary>
        /// <param name="byteCount">The total length in bytes of the endcoded string.</param>
        public static string ReadString(byte[] data, int offset, int byteCount, Encoding encoding)
        {
            if (byteCount <= 0)
                return string.Empty;

            if ((uint)offset + byteCount > data.Length)
                throw new ArgumentOutOfRangeException("ByteCount + Offset is larger than data length.");

            fixed (byte* ptr = data)
            {
                return ReadString((IntPtr)ptr, offset, byteCount, encoding);
            }
        }

        /// <summary>
        /// Reads a string from the supplied buffer.
        /// </summary>
        /// <param name="byteCount">The total length in bytes of the endcoded string.</param>
        public static string ReadString(IntPtr data, int offset, int byteCount, Encoding encoding)
        {
            return new string((sbyte*)data, offset, byteCount, encoding);
        }
    }
}
