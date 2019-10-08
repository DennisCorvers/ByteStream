using ByteStream.Mananged;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStreamTest.Managed
{
    [TestFixture]
    public class ByteReaderTest
    {
        [Test]
        public void CTorTest()
        {
            byte[] buf = new byte[30];

            Assert.Catch(typeof(ArgumentNullException), () =>
            { var br = new ByteReader(null); });

            ByteReader reader = new ByteReader(buf);
            Assert.AreEqual(30, reader.Length);
            Assert.AreEqual(0, reader.Offset);
            Assert.AreEqual(true, reader.IsFixedSize);
        }

        [Test]
        public void CTorFaultTest()
        {
            byte[] buf = new byte[30];

            ByteReader br;
            Assert.Catch(typeof(ArgumentNullException), () =>
            { br = new ByteReader(null); });
            Assert.Catch(typeof(ArgumentOutOfRangeException), () =>
            { br = new ByteReader(buf, -1, -1); });
            Assert.Catch(typeof(ArgumentOutOfRangeException), () =>
            { br = new ByteReader(buf, 0, -1); });
            Assert.Catch(typeof(ArgumentException), () =>
            { br = new ByteReader(buf, 25, 6); });
        }

        [Test]
        public void CTorOffsetTest()
        {
            byte[] buf = new byte[30];
            ByteReader br = new ByteReader(buf, 10, 15);
            Assert.AreEqual(10, br.Offset);
            Assert.AreEqual(15, br.Length);
        }

        [Test]
        public void SkipBytesTest()
        {
            var br = new ByteReader(new byte[10]);
            Assert.Catch(typeof(InvalidOperationException), () => { br.SkipBytes(11); });
            br.SkipBytes(10);
            Assert.AreEqual(10, br.Offset);
        }

        [Test]
        public void ClearTest()
        {
            var br = new ByteReader(new byte[10], 5, 5);
            br.Clear();
            Assert.AreEqual(0, br.Offset);
        }
    }
}
