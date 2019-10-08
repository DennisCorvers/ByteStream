using ByteStream;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamTest
{
    [TestFixture]
    public class StringHelperTest
    {
        private class IntPtrTest
        {
            private readonly IntPtr m_buffer;

            public IntPtrTest()
            {
                m_buffer = Marshal.AllocHGlobal(64);
            }
            ~IntPtrTest()
            {
                if (m_buffer != IntPtr.Zero)
                { Marshal.FreeHGlobal(m_buffer); }
            }

            [TestCase("手機瀏覽")]
            public void UTF16Test(string value)
            {
                int length = value.Length * sizeof(char);

                for (int i = 0; i < 4 * length; i += length)
                { StringHelper.WriteUTF16(m_buffer, i, value); }

                for (int i = 0; i < 4 * length; i += length)
                { Assert.AreEqual(value, StringHelper.ReadUTF16(m_buffer, i, value.Length)); }
            }

            [TestCase("Test.")]
            public void ANSITest(string value)
            {
                int length = value.Length;

                for (int i = 0; i < 4 * length; i += length)
                { StringHelper.WriteANSI(m_buffer, i, value); }

                for (int i = 0; i < 4 * length; i += length)
                { Assert.AreEqual(value, StringHelper.ReadANSI(m_buffer, i, value.Length)); }
            }
        }
        private class ByteArrTest
        {
            private readonly byte[] m_buffer;

            public ByteArrTest()
            {
                m_buffer = new byte[64];
            }

            [TestCase("手機瀏覽")]
            public void UTF16Test(string value)
            {
                int length = value.Length * sizeof(char);

                for (int i = 0; i < 4 * length; i += length)
                { StringHelper.WriteUTF16(m_buffer, i, value); }

                for (int i = 0; i < 4 * length; i += length)
                { Assert.AreEqual(value, StringHelper.ReadUTF16(m_buffer, i, value.Length)); }
            }

            [TestCase("Test.")]
            public void ANSITest(string value)
            {
                int length = value.Length;

                for (int i = 0; i < 4 * length; i += length)
                { StringHelper.WriteANSI(m_buffer, i, value); }

                for (int i = 0; i < 4 * length; i += length)
                { Assert.AreEqual(value, StringHelper.ReadANSI(m_buffer, i, value.Length)); }
            }
        }
    }
}
