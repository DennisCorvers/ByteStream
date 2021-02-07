using System;

namespace ByteStream
{
    public unsafe static class StringHelper
    {
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
    }
}
