using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ByteStream.Mananged;
using ByteStream;
using System.Runtime.InteropServices;
using ByteStream.Unmanaged;

namespace ByteStreamTest.Unmanaged
{
    public class ByteStreamTests
    {
        private const int BUFSIZE = 32;
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

        [Test]
        public void ReadWriteTest()
        {
            UnmanagedStream bs = new UnmanagedStream();

            bs.ResetWrite(m_buffer, BUFSIZE);

            bs.Write(123);

            bs.ResetRead();

            int res = 0;
            bs.Read(ref res);
            Assert.AreEqual(123, res);
        }

        [Test]
        public void ResetReadFault()
        {
            UnmanagedStream bs = new UnmanagedStream();

            Assert.Catch<InvalidOperationException>(() => { bs.ResetRead(); });
        }

        [Test]
        public void InvalidateStreamer()
        {
            UnmanagedStream bs = new UnmanagedStream();
            bs.ResetWrite(m_buffer, 4);

            bs.Write<int>(123);

            Assert.Catch<InvalidOperationException>(() =>
            {
                bs.Write<int>(321);
            });
        }

        [Test]
        public void InvalidateStream()
        {
            UnmanagedStream bs = new UnmanagedStream();
            bs.ResetWrite(m_buffer, 2);

            Assert.Catch<InvalidOperationException>(() =>
            {
                bs.Write<int>(321);
            });

            Assert.AreEqual(2, bs.Length);
        }

        [Test]
        public void ResetStreamer()
        {
            UnmanagedStream bs = new UnmanagedStream();
            bs.ResetWrite(m_buffer, 8);

            bs.Write<int>(333);

            Assert.AreEqual(4, bs.Offset);
            Assert.IsTrue(bs.IsWriting);

            bs.ResetRead();

            Assert.AreEqual(0, bs.Offset);
            Assert.IsTrue(bs.IsReading);
        }

        [Test]
        public void LargeStringWrite()
        {
            UnmanagedStream ms = new UnmanagedStream();
            ms.ResetWrite(Marshal.AllocHGlobal(128), 128);

            StringBuilder sb = new StringBuilder(50);
            for (int i = 0; i < 50; i++)
                sb.Append(i);

            var str = sb.ToString();

            ms.WriteString(str, Encoding.Default);

            Assert.IsTrue(ms.Length >= 64);

            ms.ResetRead();

            Assert.AreEqual(str, ms.ReadString(Encoding.Default));

            Marshal.FreeHGlobal(ms.Buffer);
        }
    }
}
