using ByteStream;
using ByteStream.Mananged;
using ByteStream.Unmanaged;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamTest.Unmanaged
{
    [TestFixture]
    public class PtrReadWriteTest
    {
        //Do not change this value.
        private const int BUFSIZE = 64;
        private IntPtr m_buffer;

        [SetUp]
        public void Init()
        {
            m_buffer = Marshal.AllocHGlobal(BUFSIZE);
        }
        [TearDown]
        public void TearDown()
        {
            Marshal.FreeHGlobal(m_buffer);
        }

        [TestCase(123, 1.2f, 1.3d)]
        public void ValueTest(int iVal, float fVal, double dVal)
        {
            var pw = new PtrWriter(m_buffer, 64);
            pw.Write(iVal); pw.Write(fVal); pw.Write(dVal);

            var pr = new PtrReader(m_buffer, 64);
            Assert.AreEqual(iVal, pr.Read<int>());
            Assert.AreEqual(fVal, pr.Read<float>());
            Assert.AreEqual(dVal, pr.Read<double>());

            Assert.AreEqual(16, pw.Offset);
            Assert.AreEqual(pr.Offset, pw.Offset);
        }

        [Test]
        public void TryWrite()
        {
            var bw = new PtrWriter(m_buffer, 64);
            Assert.AreEqual(true, bw.TryWrite(123));

            bw.SkipBytes(60);
            Assert.AreEqual(false, bw.TryWrite(123));
        }

        [Test]
        public void TryRead()
        {
            var br = new PtrReader(m_buffer, 64);
            Assert.AreEqual(true, br.TryRead(out int value));

            br.SkipBytes(60);
            Assert.AreEqual(false, br.TryRead(out int valu2));
        }

        [Test]
        public void BytesTest()
        {
            byte[] val = new byte[4] { 1, 2, 3, 4 };
            var pw = new PtrWriter(m_buffer, 64);
            pw.WriteBytes(val);

            var pr = new PtrReader(m_buffer, 64);
            var returnVal = pr.ReadBytes(4);

            Assert.AreEqual(val, returnVal);
            Assert.AreEqual(4, pr.Offset);
            Assert.AreEqual(pw.Offset, pr.Offset);
        }

        [TestCase("手機瀏覽")]
        public void UTF16Test(string value)
        {
            var pw = new PtrWriter(m_buffer, 64);
            pw.WriteUTF16(value, true);

            var pr = new PtrReader(m_buffer, 64);
            Assert.AreEqual(value, pr.ReadUTF16());
            Assert.AreEqual(pr.Offset, pw.Offset);
        }

        [TestCase("手機瀏覽")]
        public void UTF16TestLen(string value)
        {
            var pw = new PtrWriter(m_buffer, 64);
            pw.WriteUTF16(value);

            var pr = new PtrReader(m_buffer, 64);
            Assert.AreEqual(value, pr.ReadUTF16(value.Length));
            Assert.AreEqual(pr.Offset, pw.Offset);
            Assert.AreEqual(value.Length * sizeof(char), pw.Offset);
        }

        [TestCase("Test.")]
        public void ANSITestLen(string value)
        {
            var pw = new PtrWriter(m_buffer, 64);
            pw.WriteANSI(value);

            var pr = new PtrReader(m_buffer, 64);
            Assert.AreEqual(value, pr.ReadANSI(value.Length));
            Assert.AreEqual(pr.Offset, pw.Offset);
            Assert.AreEqual(value.Length, pw.Offset);
        }
    }
}
