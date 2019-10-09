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
    public class PtrReaderTest
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
        public void CTorTest()
        {
            byte[] buf = new byte[30];

            Assert.Catch(typeof(ArgumentNullException), () =>
            { var br = new PtrReader(IntPtr.Zero, 64); });

            PtrReader reader = new PtrReader(m_buffer, 64);
            Assert.AreEqual(64, reader.Length);
            Assert.AreEqual(0, reader.Offset);
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
        public void CTorOffsetTest()
        {
            PtrReader br = new PtrReader(m_buffer, 10, 15);
            Assert.AreEqual(10, br.Offset);
            Assert.AreEqual(15, br.Length);
        }

        [Test]
        public void SkipBytesTest()
        {
            var br = new PtrReader(m_buffer, 10);
            Assert.Catch(typeof(InvalidOperationException), () => { br.SkipBytes(11); });
            br.SkipBytes(10);
            Assert.AreEqual(10, br.Offset);
        }

        [Test]
        public void ClearTest()
        {
            var br = new PtrReader(m_buffer, 5, 5);
            br.Clear();
            Assert.AreEqual(0, br.Offset);
        }
    }
}
