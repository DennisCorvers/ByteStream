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
            byte[] buf = new byte[16];
            ByteWriterFast bwf = new ByteWriterFast(buf);
            bwf.Pin();
            bwf.Write(123);
            bwf.UnPin();

            
        }
    }
}
