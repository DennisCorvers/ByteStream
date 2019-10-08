using ByteStream.Unmanaged;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStreamTest.Unmanaged
{
    [TestFixture]
    public class ByteWriterFTest
    {
        [Test]
        public void CtorTest()
        {
            byte[] buf = new byte[1024];
            ByteWriterFast bwf = new ByteWriterFast(buf);
            int size = System.Runtime.InteropServices.Marshal.SizeOf<ByteWriterFast>();

            for (int i = 0; i < 64; i++)
            {

            }
            bwf.Pin();
            bwf.Write(123);
            bwf.UnPin();
            bwf.Pin();
            bwf.Write(123);
            bwf.UnPin();
        }
    }
}
