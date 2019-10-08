using ByteStream;
using ByteStream.Mananged;
using ByteStream.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamTest.Managed
{
    [TestFixture]
    public class ByteWriterTest
    {
        [Test]
        public void CTorTest()
        {
            byte[] buf = new byte[30];

            Assert.Catch(typeof(ArgumentNullException), () =>
            { var br = new ByteWriter(null); });

            ByteWriter writer = new ByteWriter(buf);
            Assert.AreEqual(30, writer.Length);
            Assert.AreEqual(0, writer.Offset);
            Assert.AreEqual(true, writer.IsFixedSize);
        }

        [Test]
        public void CTorFaultTest()
        {
            byte[] buf = new byte[30];

            ByteWriter bw;
            Assert.Catch(typeof(ArgumentNullException), () =>
            { bw = new ByteWriter(null); });
            Assert.Catch(typeof(ArgumentException), () =>
            { bw = new ByteWriter(buf, 40); });
        }

        [Test]
        public void CTorNewTest()
        {
            ByteWriter bw = new ByteWriter(30, true);
            Assert.AreEqual(30, bw.Length);
        }

        [Test]
        public void CTorOffsetTest()
        {
            byte[] buf = new byte[30];
            ByteWriter bw = new ByteWriter(buf, 10);
            Assert.AreEqual(10, bw.Offset);
            Assert.AreEqual(30, bw.Length);
        }

        [Test]
        public void SkipBytesTest()
        {
            var bw = new ByteWriter(new byte[10]);
            Assert.Catch(typeof(InvalidOperationException), () => { bw.SkipBytes(11); });
            bw.SkipBytes(10);
            Assert.AreEqual(10, bw.Offset);
        }

        [Test]
        public void ClearTest()
        {
            var bw = new ByteWriter(new byte[10], 5);
            bw.Clear();
            Assert.AreEqual(0, bw.Offset);
        }

        [Test]
        public void ExpandTest()
        {
            ByteWriter bw = new ByteWriter(10, false);
            bw.SkipBytes(11);
            Assert.AreEqual(16, bw.Length);
            Assert.AreEqual(11, bw.Offset);

            bw.SkipBytes(100);
            Assert.AreEqual(116.NextPowerOfTwo(), bw.Length);
        }

        [Test]
        public void TrimTest()
        {
            ByteWriter bw = new ByteWriter(new byte[16], 10);
            bw.Trim();
            Assert.AreEqual(10, bw.Length);
            Assert.AreEqual(bw.Offset, bw.Length);
        }

        [TestCase(280851)]
        public void CopyToBytesTest(int value)
        {
            byte[] buf = new byte[8];
            BinaryHelper.Write(buf, 0, value);

            var writer = new ByteWriter(buf);
            var newBuf = new byte[4];
            writer.CopyTo(newBuf, 0, 4);

            Assert.AreEqual(value, BinaryHelper.Read<int>(newBuf, 0));
        }

        //[TestCase(1598423)]
        //public void CopyToPtrTest(int value)
        //{
        //    byte[] buf = new byte[8];
        //    BinaryHelper.Write(buf, 0, value);

        //    var writer = new ByteWriter(buf);
        //    var newBuf = Marshal.AllocHGlobal(4);
        //    writer.CopyTo(newBuf, 0, 4);

        //    Assert.AreEqual(value, BinaryHelper.Read<int>(newBuf, 0));
        //    Marshal.FreeHGlobal(newBuf);
        //}
    }
}
