using ByteStream;
using ByteStream.Mananged;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamTest.Managed
{
    [TestFixture]
    public class ByteReadWriteTest
    {
        //Do not change this value!
        private const int BUFSIZE = 32;

        private byte[] m_buffer;

        [SetUp]
        public void Init()
        {
            m_buffer = new byte[BUFSIZE];
        }

        [TestCase(123, 1.2f, 1.3d)]
        public void ValueTest(int iVal, float fVal, double dVal)
        {
            var bw = new ByteWriter(m_buffer);
            bw.Write(iVal); bw.Write(fVal); bw.Write(dVal);

            var br = new ByteReader(m_buffer);
            Assert.AreEqual(iVal, br.Read<int>());
            Assert.AreEqual(fVal, br.Read<float>());
            Assert.AreEqual(dVal, br.Read<double>());

            Assert.AreEqual(16, bw.Offset);
            Assert.AreEqual(br.Offset, bw.Offset);
        }

        [Test]
        public void TryWrite()
        {
            var bw = new ByteWriter(m_buffer);
            Assert.AreEqual(true, bw.TryWrite(123));

            bw.SkipBytes(28);
            Assert.AreEqual(false, bw.TryWrite(123));
        }

        [Test]
        public void TryRead()
        {
            var br = new ByteReader(m_buffer);
            Assert.AreEqual(true, br.TryRead(out int value));

            br.SkipBytes(28);
            Assert.AreEqual(false, br.TryRead(out int valu2));
        }

        [Test]
        public void BytesTest()
        {
            byte[] val = new byte[4] { 1, 2, 3, 4 };
            var bw = new ByteWriter(m_buffer);
            bw.WriteBytes(val);

            var br = new ByteReader(m_buffer);
            var returnVal = br.ReadBytes(4);

            Assert.AreEqual(val, returnVal);
            Assert.AreEqual(4, br.Offset);
            Assert.AreEqual(bw.Offset, br.Offset);
        }

        [TestCase("手機瀏覽")]
        public void UTF16Test(string value)
        {
            var bw = new ByteWriter(m_buffer);
            bw.WriteUTF16(value, true);

            var br = new ByteReader(m_buffer);
            Assert.AreEqual(value, br.ReadUTF16());
            Assert.AreEqual(br.Offset, bw.Offset);
        }

        [TestCase("手機瀏覽")]
        public void UTF16TestLen(string value)
        {
            var bw = new ByteWriter(m_buffer);
            bw.WriteUTF16(value);

            var br = new ByteReader(m_buffer);
            Assert.AreEqual(value, br.ReadUTF16(value.Length));
            Assert.AreEqual(br.Offset, bw.Offset);
            Assert.AreEqual(value.Length * sizeof(char), bw.Offset);
        }

        [TestCase("Test.")]
        public void ANSITestLen(string value)
        {
            var bw = new ByteWriter(m_buffer);
            bw.WriteANSI(value);

            var br = new ByteReader(m_buffer);
            Assert.AreEqual(value, br.ReadANSI(value.Length));
            Assert.AreEqual(br.Offset, bw.Offset);
            Assert.AreEqual(value.Length, bw.Offset);
        }

        [TestCase("手機瀏覽")]
        [TestCase("Test.")]
        public void StringTest(string value)
        {
            var bw = new ByteWriter(m_buffer);
            bw.WriteString(value, Encoding.UTF32);

            var br = new ByteReader(m_buffer);

            Assert.AreEqual(value, br.ReadString(Encoding.UTF32));
            Assert.AreEqual(bw.Offset, br.Offset);
        }
    }
}
