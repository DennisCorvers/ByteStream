using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStream
{
    public unsafe static class StringHelper
    {
        /// <summary>
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadString(byte[] bin, int offset, int stringLength)
        {
            if (stringLength < 1) { return string.Empty; }

            fixed (byte* ptr = &bin[offset])
            { return new string((char*)ptr, 0, stringLength); }
        }
        /// <summary>
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadString(IntPtr data, int offset, int stringLength)
        {
            if (stringLength < 1) { return string.Empty; }
            return new string((char*)data, offset, stringLength);
        }

        /// <summary>
        /// Reads a string in ASCII encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadASCII(byte[] bin, int offset, int stringLength)
        {
            if (stringLength < 1) { return string.Empty; }
            fixed (byte* ptr = bin)
            { return new string((sbyte*)ptr, offset, stringLength); }
        }
        /// <summary>
        /// Reads a string in ASCII encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadASCII(IntPtr data, int offset, int stringLength)
        {
            if (stringLength < 1) { return string.Empty; }
            return new string((sbyte*)data, offset, stringLength);
        }

        /// <summary>
        /// Reads a string in UTF8 encoding.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="byteSize">The size of the string in bytes.</param>
        public static string ReadUTF8(byte[] bin, int offset, int byteSize)
        {
            if (byteSize < 1) { return string.Empty; }
            return Encoding.UTF8.GetString(bin, offset, byteSize);
        }
        /// <summary>
        /// Reads a string in UTF8 encoding. Length is automatically retrieved as an uint16.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="byteSize">The length of the string in bytes.</param>
        public static string ReadUTF8(IntPtr data, int offset, int byteSize)
        {
            if (byteSize < 1) { return string.Empty; }
            return Encoding.UTF8.GetString((byte*)(data + offset), byteSize);
        }

        /// <summary>
        /// Reads a string in UTF16 encoding.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="byteSize">The size of the string in bytes.</param>
        public static string ReadUTF16(byte[] bin, int offset, int byteSize)
        {
            if (byteSize < 1) { return string.Empty; }
            return Encoding.Unicode.GetString(bin, offset, byteSize);
        }
        /// <summary>
        /// Reads a string in UTF16 encoding. Length is automatically retrieved as an uint16.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="byteSize">The length of the string in bytes.</param>
        public static string ReadUTF16(IntPtr data, int offset, int byteSize)
        {
            if (byteSize < 1) { return string.Empty; }
            return Encoding.Unicode.GetString((byte*)(data + offset), byteSize);
        }


        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteString(byte[] dest, int offset, string value)
        {
            int length = value.Length * sizeof(char);

            fixed (byte* ptr = &dest[offset])
            fixed (char* str = value)
            { Buffer.MemoryCopy(str, ptr, length, length); }
        }
        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteString(IntPtr dest, int offset, string value)
        {
            for (int i = 0; i < value.Length; i++)
            { *((char*)dest + offset + i) = value[i]; }
        }

        /// <summary>
        /// Writes a string in ASCII encoding. Each character requires 1 byte.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteASCII(byte[] dest, int offset, string value)
        {
            int length = value.Length;
            for (int i = 0; i < length; i++)
            { dest[offset + i] = (byte)value[i]; }
        }
        /// <summary>
        /// Writes a string in ASCII encoding. Each character requires 1 byte.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteASCII(IntPtr dest, int offset, string value)
        {
            for (int i = 0; i < value.Length; i++)
            { *((byte*)dest + offset + i) = (byte)value[i]; }
        }

        /// <summary>
        /// Writes a string in UTF8 encoding. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteUTF8(byte[] dest, int offset, string value)
        {
            Encoding.UTF8.GetBytes(value, 0, value.Length, dest, offset);
        }
        /// <summary>
        /// Writes a string in UTF8 encoding.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        /// <param name="byteSize">The length of the encoded string in bytes.</param>
        public static void WriteUTF8(IntPtr dest, int offset, string value, int byteSize)
        {
            fixed (char* ptr = value)
            { Encoding.UTF8.GetBytes(ptr, value.Length, (byte*)(dest + offset), byteSize); }
        }

        /// <summary>
        /// Writes a string in UTF16 encoding. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteUTF16(byte[] dest, int offset, string value)
        {
            Encoding.Unicode.GetBytes(value, 0, value.Length, dest, offset);
        }
        /// <summary>
        /// Writes a string in UTF16 encoding. Includes the length of the string as an uint16.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        /// <param name="byteSize">The length of the encoded string in bytes.</param>
        public static void WriteUTF16(IntPtr dest, int offset, string value, int byteSize)
        {
            fixed (char* ptr = value)
            { Encoding.Unicode.GetBytes(ptr, value.Length, (byte*)(dest + offset), byteSize); }
        }
    }
}
