using ByteStream;
using ByteStream.Mananged;
using ByteStream.Unmanaged;
using ByteStream.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamTest.Unmanaged
{
    [TestFixture]
    public class PtrWriterTest
    {
        private IntPtr m_buffer;

        [SetUp]
        public void Init()
        {
            m_buffer = Marshal.AllocHGlobal(64);
        }
        [TearDown]
        public void TearDown()
        {
            Marshal.FreeHGlobal(m_buffer);
        }

        [Test]
        public void CTorFaultTest()
        {
            PtrWriter pw;
            Assert.Catch(typeof(ArgumentNullException), () =>
            { pw = new PtrWriter(IntPtr.Zero, 64); });
            Assert.Catch(typeof(ArgumentOutOfRangeException), () =>
            { pw = new PtrWriter(m_buffer, -1, -1); });
            Assert.Catch(typeof(ArgumentOutOfRangeException), () =>
            { pw = new PtrWriter(m_buffer, 0, -1); });
            Assert.Catch(typeof(ArgumentException), () =>
            { pw = new PtrWriter(m_buffer, 25, 6); });
        }

        [Test]
        public void CTorTest()
        {
            PtrWriter writer = new PtrWriter(m_buffer, 64);
            Assert.AreEqual(64, writer.Length);
            Assert.AreEqual(0, writer.Offset);
        }

        [Test]
        public void SkipBytesTest()
        {
            var pw = new PtrWriter(m_buffer, 10);
            Assert.Catch(typeof(InvalidOperationException), () => { pw.SkipBytes(11); });
            pw.SkipBytes(10);
            Assert.AreEqual(10, pw.Offset);
        }

        [Test]
        public void ClearTest()
        {
            var pw = new PtrWriter(m_buffer, 5, 64);
            pw.Clear();
            Assert.AreEqual(0, pw.Offset);
        }

        [TestCase(280851)]
        public void CopyToBytesTest(int value)
        {
            byte[] buf = new byte[8];

            BinaryHelper.Write(m_buffer, 4, value);
            var pw = new PtrWriter(m_buffer, 64);
            pw.CopyTo(buf, 0, 8);

            Assert.AreEqual(value, BinaryHelper.Read<int>(buf, 4));
        }

        [TestCase(1337666)]
        public void CopyToPtrTest(int value)
        {
            BinaryHelper.Write(m_buffer, 4, value);

            var pw = new PtrWriter(m_buffer, 64);
            var newBuf = Marshal.AllocHGlobal(8);
            pw.CopyTo(newBuf, 0, 8);

            Assert.AreEqual(value, BinaryHelper.Read<int>(newBuf, 4));
            Marshal.FreeHGlobal(newBuf);
        }
    }
}
