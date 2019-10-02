using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStream
{
    public unsafe static class BinaryHelper
    {
        #region ReadBytes
        /// <summary>
        /// Reads a boolean value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBool(byte[] bin, int offset)
        {
            return bin[offset] == 1;
        }
        /// <summary>
        /// Reads a 8-bit unsigned integer value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(byte[] bin, int offset)
        {
            return bin[offset];
        }
        /// <summary>
        /// Reads a 8-bit signed integer value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(byte[] bin, int offset)
        {
            return (sbyte)bin[offset];
        }
        /// <summary>
        /// Reads a char value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(byte[] bin, int offset)
        {
            fixed (byte* pbyte = &bin[offset])
            { return *(char*)pbyte; }
        }
        /// <summary>
        /// Reads a char value from the source array with a size of one byte.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadCharSingle(byte[] bin, int offset)
        {
            return (char)bin[offset];
        }
        /// <summary>
        /// Reads a decimal value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(decimal*)ptr; }
        }
        /// <summary>
        /// Reads a double precision float value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(double*)ptr; }
        }
        /// <summary>
        /// Reads a single precision float value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(float*)ptr; }
        }
        /// <summary>
        /// Reads a 32-bit integer value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(int*)ptr; }
        }
        /// <summary>
        /// Reads a 32-bit unsigned integer value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(uint*)ptr; }
        }
        /// <summary>
        /// Reads a 64-bit integer value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLong(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(long*)ptr; }
        }
        /// <summary>
        /// Reads a 64-bit unsigned integer value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadULong(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(ulong*)ptr; }
        }
        /// <summary>
        /// Reads a 16-bit integer value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShort(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(short*)ptr; }
        }
        /// <summary>
        /// Reads a 16-bit unsigned integer value from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUShort(byte[] bin, int offset)
        {
            fixed (byte* ptr = &bin[offset])
            { return *(ushort*)ptr; }
        }

        /// <summary>
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadDBCS(byte[] bin, int offset, int stringLength)
        {
            if (stringLength < 1) { return string.Empty; }

            fixed (byte* ptr = &bin[offset])
            { return new string((char*)ptr, 0, stringLength); }
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
        /// Reads a string in DBCS encoding to match a certain length.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of this string.</param>
        public static string ReadDBSCPadded(byte[] bin, int offset, int stringLength)
        {
            if (stringLength < 1) { return string.Empty; }

            fixed (byte* ptr = &bin[offset])
            { return new string((char*)ptr, 0, stringLength); }
        }
        /// <summary>
        /// Reads a string in ASCII encoding to match a certain length.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of this string.</param>
        public static string ReadASCIIPadded(byte[] bin, int offset, int stringLength)
        {
            if (stringLength < 1) { return string.Empty; }

            fixed (byte* ptr = bin)
            { return new string((sbyte*)ptr, offset, stringLength); }
        }

        /// <summary>
        /// Reads a byte-array from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        /// <param name="length">The total length of the byte buffer to read as an int32</param>
        public static byte[] ReadBytes(byte[] bin, int offset, int length)
        {
            if (length < 1) { return new byte[0]; }

            byte[] result = new byte[length];

            bin.CopyUnsafe(offset, result, 0, length);
            return result;
        }
        /// <summary>
        /// Reads a blittable value type from the source array.
        /// </summary>
        /// <param name="bin">The source array.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadValue<T>(byte[] bin, int offset) where T : unmanaged
        {
            fixed (byte* ptr = &bin[offset])
            { return *(T*)ptr; }
        }
        #endregion

        #region WriteBytes
        /// <summary>
        /// Writes a boolean value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBool(byte[] dest, int offset, bool value)
        {
            dest[offset] = (byte)(value ? 1 : 0);
        }
        /// <summary>
        /// Writes a 8-bit integer value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSByte(byte[] dest, int offset, sbyte value)
        {
            dest[offset] = (byte)value;
        }
        /// <summary>
        /// Writes a 8-bit unsigned integer value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteByte(byte[] dest, int offset, byte value)
        {
            dest[offset] = value;
        }
        /// <summary>
        /// Writes a char in 2 bytes to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteChar(byte[] dest, int offset, char value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(char*)ptr = value; }
        }
        /// <summary>
        /// Writes a char in a single byte to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCharSingle(byte[] dest, int offset, char value)
        {
            dest[offset] = (byte)value;
        }
        /// <summary>
        /// Writes a double precision point value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDouble(byte[] dest, int offset, double value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(double*)ptr = value; }
        }
        /// <summary>
        /// Writes a single floating precision point value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSingle(byte[] dest, int offset, float value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(float*)ptr = value; }
        }
        /// <summary>
        /// Wries a decimal value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDecimal(byte[] dest, int offset, decimal value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(decimal*)ptr = value; }
        }
        /// <summary>
        /// Writes a 32-bit integer value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt(byte[] dest, int offset, int value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(int*)ptr = value; }
        }
        /// <summary>
        /// Writes a 32-bit unsigned integer value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt(byte[] dest, int offset, uint value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(uint*)ptr = value; }
        }
        /// <summary>
        /// Writes a 64-bit signed integer value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLong(byte[] dest, int offset, long value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(long*)ptr = value; }
        }
        /// <summary>
        /// Writes a 64-bit unsigned integer value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteULong(byte[] dest, int offset, ulong value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(ulong*)ptr = value; }
        }
        /// <summary>
        /// Writes a 16-bit signed integer value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteShort(byte[] dest, int offset, short value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(short*)ptr = value; }
        }
        /// <summary>
        /// Writes a 16-bit unsigned integer value to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUshort(byte[] dest, int offset, ushort value)
        {
            fixed (byte* ptr = &dest[offset])
            { *(ushort*)ptr = value; }
        }

        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteDBCS(byte[] dest, int offset, string value)
        {
            int length = value.Length * sizeof(char);

            fixed (byte* ptr = &dest[offset])
            fixed (char* str = value)
            { Buffer.MemoryCopy(str, ptr, length, length); }
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
        /// Writes a byte array to the specified array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteBytes(byte[] dest, int offset, byte[] value)
        {
            value.CopyUnsafe(0, dest, offset, value.Length);
        }
        /// <summary>
        /// Clears a given number of bytes.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="bytesToSkip">The amount of bytes to clear.</param>
        public static void ClearBytes(byte[] dest, int offset, int bytesToSkip)
        {
            if (bytesToSkip < 1) { throw new ArgumentOutOfRangeException("bytesToSkip", "Must be at least 1."); }
            for (int i = 0; i < bytesToSkip; i++)
            { dest[i + offset] = 0; }
        }
        /// <summary>
        /// Writes a blittable value type to the specified byte array.
        /// </summary>
        /// <param name="dest">The destination byte array.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteValue<T>(byte[] dest, int offset, T value) where T : unmanaged
        {
            fixed (byte* ptr = &dest[offset])
            { *(T*)ptr = value; }
        }
        #endregion

        #region ReadSpan
        /// <summary>
        /// Reads a boolean value from the specified memory.
        /// </summary>
        /// <param name="data">The destination memory.</param>
        /// <param name="offset">The current read offset.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBool(IntPtr data, int offset)
        {
            return *((byte*)data + offset) > 0;
        }
        /// <summary>
        /// Reads a 8-bit unsigned integer value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(IntPtr data, int offset)
        {
            return *((byte*)data + offset);
        }
        /// <summary>
        /// Reads a 8-bit signed integer value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(IntPtr data, int offset)
        {
            return *((sbyte*)data + offset);
        }
        /// <summary>
        /// Reads a char value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(IntPtr data, int offset)
        {
            return *((char*)data + offset);
        }
        /// <summary>
        /// Reads a char value from the source memory with a size of one byte.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadCharSingle(IntPtr data, int offset)
        {
            return (char)*((byte*)data + offset);
        }
        /// <summary>
        /// Reads a decimal value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(IntPtr data, int offset)
        {
            return *((decimal*)data + offset);
        }
        /// <summary>
        /// Reads a double precision float value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(IntPtr data, int offset)
        {
            return *((double*)data + offset);
        }
        /// <summary>
        /// Reads a single precision float value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(IntPtr data, int offset)
        {
            return *((float*)data + offset);
        }
        /// <summary>
        /// Reads a 32-bit integer value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt(IntPtr data, int offset)
        {
            return *((byte*)data + offset);
        }
        /// <summary>
        /// Reads a 32-bit unsigned integer value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt(IntPtr data, int offset)
        {
            return *((uint*)data + offset);
        }
        /// <summary>
        /// Reads a 64-bit integer value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLong(IntPtr data, int offset)
        {
            return *((long*)data + offset);
        }
        /// <summary>
        /// Reads a 64-bit unsigned integer value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadULong(IntPtr data, int offset)
        {
            return *((ulong*)data + offset);
        }
        /// <summary>
        /// Reads a 16-bit integer value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShort(IntPtr data, int offset)
        {
            return *((short*)data + offset);
        }
        /// <summary>
        /// Reads a 16-bit unsigned integer value from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUShort(IntPtr data, int offset)
        {
            return *((ushort*)data + offset);
        }

        /// <summary>
        /// Reads a string as a double-byte character set. Each character requires 2 bytes.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="stringLength">The total length of the string to read.</param>
        public static string ReadDBCS(IntPtr data, int offset, int stringLength)
        {
            if (stringLength < 1) { return string.Empty; }
            return new string((char*)data, offset, stringLength);
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
        /// Reads a byte-memory from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The offset at which to read.</param>
        /// <param name="length">The total length of the byte buffer to read as an int32</param>
        public static byte[] ReadBytes(IntPtr data, int offset, int length)
        {
            if (length < 1) { return new byte[0]; }

            byte[] result = new byte[length];

            fixed (byte* ptr = result)
            { Buffer.MemoryCopy((byte*)data + offset, ptr, length, length); }

            return result;
        }
        /// <summary>
        /// Reads a blittable value type from the source memory.
        /// </summary>
        /// <param name="data">The source memory.</param>
        /// <param name="offset">The current read offset.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadValue<T>(IntPtr data, int offset) where T : unmanaged
        {
            return *((T*)data + offset);
        }
        #endregion

        #region WriteSpan
        /// <summary>
        /// Writes a boolean value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBool(IntPtr dest, int offset, bool value)
        {
            *((byte*)dest + offset) = (byte)(value ? 1 : 0);
        }
        /// <summary>
        /// Writes a 8-bit integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSByte(IntPtr dest, int offset, sbyte value)
        {
            *((sbyte*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a 8-bit unsigned integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memoryy.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteByte(IntPtr dest, int offset, byte value)
        {
            *((byte*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a char in 2 bytes to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteChar(IntPtr dest, int offset, char value)
        {
            *((char*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a char in a single byte to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteCharSingle(IntPtr dest, int offset, char value)
        {
            *((byte*)dest + offset) = (byte)value;
        }
        /// <summary>
        /// Writes a double precision point value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDouble(IntPtr dest, int offset, double value)
        {
            *((double*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a single floating precision point value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteSingle(IntPtr dest, int offset, float value)
        {
            *((float*)dest + offset) = value;
        }
        /// <summary>
        /// Wries a decimal value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDecimal(IntPtr dest, int offset, decimal value)
        {
            *((decimal*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a 32-bit integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt(IntPtr dest, int offset, int value)
        {
            *((int*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a 32-bit unsigned integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt(IntPtr dest, int offset, uint value)
        {
            *((uint*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a 64-bit signed integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLong(IntPtr dest, int offset, long value)
        {
            *((long*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a 64-bit unsigned integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteULong(IntPtr dest, int offset, ulong value)
        {
            *((ulong*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a 16-bit signed integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteShort(IntPtr dest, int offset, short value)
        {
            *((short*)dest + offset) = value;
        }
        /// <summary>
        /// Writes a 16-bit unsigned integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUshort(IntPtr dest, int offset, ushort value)
        {
            *((ushort*)dest + offset) = value;
        }

        /// <summary>
        /// Writes a string as a double-byte character set. Each character requires 2 bytes.
        /// Does NOT include the length.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static void WriteDBCS(IntPtr dest, int offset, string value)
        {
            for (int i = 0; i < value.Length; i++)
            { *((char*)dest + offset + i) = value[i]; }
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
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        /// <param name="byteSize">The length of the encoded string in bytes.</param>
        public static void WriteUTF16(IntPtr dest, int offset, string value, int byteSize)
        {
            fixed (char* ptr = value)
            { Encoding.Unicode.GetBytes(ptr, value.Length, (byte*)(dest + offset), byteSize); }
        }

        /// <summary>
        /// Writes a byte array to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        public static unsafe void WriteBytes(IntPtr dest, int offset, byte[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            fixed (byte* src = value)
            { Buffer.MemoryCopy(src, (void*)(dest + offset), value.Length, value.Length); }
        }
        /// <summary>
        /// Skips a given number of bytes.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="bytesToSkip">The amount of bytes to clear.</param>
        public static void ClearBytes(IntPtr dest, int offset, int bytesToSkip)
        {
            if (bytesToSkip < 1) { throw new ArgumentOutOfRangeException("bytesToSkip", "Must be at least 1."); }
            if (bytesToSkip > 1024) { throw new ArgumentOutOfRangeException("bytesToSkip", "Maximum allowed size is 1024"); }

            byte* zeros = stackalloc byte[bytesToSkip];
            Buffer.MemoryCopy(zeros, (void*)(dest + offset), bytesToSkip, bytesToSkip);
        }
        /// <summary>
        /// Writes a 16-bit unsigned integer value to the specified memory.
        /// </summary>
        /// <param name="dest">The destination memory.</param>
        /// <param name="offset">The current write offset.</param>
        /// <param name="value">The value to write to the destination.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteValue<T>(IntPtr dest, int offset, T value) where T : unmanaged
        {
            *((T*)dest + offset) = value;
        }
        #endregion
    }
}
