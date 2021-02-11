using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ByteStream.Mananged;
using ByteStream;

namespace ByteStreamTest.Managed
{
    public class ByteStreamTests
    {
        private const int BUFSIZE = 32;
        private ManagedStream bs;

        private byte[] m_buffer;

        [SetUp]
        public void Init()
        {
            bs = new ManagedStream();
            m_buffer = new byte[BUFSIZE];
        }

        [Test]
        public void ReadWriteTest()
        {
            bs.ResetWrite(m_buffer);

            bs.Write(123);

            bs.ResetRead();

            int res = 0;
            bs.Read(ref res);
            Assert.AreEqual(123, res);
        }

        [Test]
        public void ResetReadFault()
        {
            ManagedStream bs = new ManagedStream();

            Assert.Catch<InvalidOperationException>(() => { bs.ResetRead(); });
        }

        [Test]
        public void ResetWriteDefault()
        {
            ManagedStream bs = new ManagedStream();

            bs.ResetWrite();

            Assert.AreEqual(SerializationMode.Writing, bs.Mode);
            Assert.IsTrue(bs.IsWriting);

            Assert.AreEqual(ManagedStream.DefaultBufferSize, bs.Length);
        }

        [Test]
        public void InvalidateStreamer()
        {
            ManagedStream bs = new ManagedStream();
            bs.ResetWrite(new byte[4], true);

            bs.Write<int>(123);

            Assert.Catch<InvalidOperationException>(() =>
            {
                bs.Write<int>(321);
            });
        }

        [Test]
        public void InvalidateStream()
        {
            ManagedStream bs = new ManagedStream();
            bs.ResetWrite(new byte[2], true);

            Assert.Catch<InvalidOperationException>(() =>
            {
                bs.Write<int>(321);
            });

            Assert.AreEqual(2, bs.Length);
        }

        [Test]
        public void ResetStreamer()
        {
            ManagedStream bs = new ManagedStream();
            bs.ResetWrite(new byte[8], true);

            bs.Write<int>(333);

            Assert.AreEqual(4, bs.Offset);
            Assert.IsTrue(bs.IsWriting);

            bs.ResetRead();

            Assert.AreEqual(0, bs.Offset);
            Assert.IsTrue(bs.IsReading);
        }

        [Test]
        public void ExpandStreamTest()
        {
            ManagedStream ms = new ManagedStream();
            ms.ResetWrite(2);

            ms.Write(123);

            // Resizes to the next power of two.
            // Which is 2 -> 4
            Assert.AreEqual(4, ms.Length);
        }

        [Test]
        public void LargeStringWrite()
        {
            ManagedStream ms = new ManagedStream();
            ms.ResetWrite(2);

            StringBuilder sb = new StringBuilder(50);
            for (int i = 0; i < 50; i++)
                sb.Append(i);

            var str = sb.ToString();

            ms.WriteString(str, Encoding.Default);

            Assert.IsTrue(ms.Length >= 64);

            ms.ResetRead();

            Assert.AreEqual(str, ms.ReadString(Encoding.Default));
        }
    }
}
