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
                Marshal.FreeHGlobal(m_buffer);
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

            [TestCase("手機瀏覽")]
            [TestCase("<Some^Sample^Text>")]
            public void EncodingTest(string value)
            {
                var encoding = Encoding.UTF7;

                int byteCount = StringHelper.WriteString(m_buffer, 0, 64, value, encoding);

                Assert.AreEqual(value, StringHelper.ReadString(m_buffer, 0, byteCount, encoding));
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

            [TestCase("手機瀏覽")]
            [TestCase("<Some^Sample^Text>")]
            public void EncodingTest(string value)
            {
                var encoding = Encoding.UTF7;

                int byteCount = StringHelper.WriteString(m_buffer, 0, value, encoding);

                Assert.AreEqual(value, StringHelper.ReadString(m_buffer, 0, byteCount, encoding));
            }

            [Test]
            public void EncodingFaultTest()
            {
                var value = @"<Some^Sample^Text>";

                // Use a buffer that is too small for the string.
                var buffer = new byte[8];
                var encoding = Encoding.UTF7;

                Assert.Catch<ArgumentException>(() =>
                {
                    StringHelper.WriteString(buffer, 0, value, encoding);
                });

                int byteCount = encoding.GetByteCount(value);

                Assert.Catch<ArgumentOutOfRangeException>(() =>
                {
                    Assert.AreEqual(value, StringHelper.ReadString(buffer, 0, byteCount, encoding));
                });
            }
        }
    }
}
