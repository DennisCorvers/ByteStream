using ByteStream.Mananged;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStreamTest.Managed
{
    [TestFixture]
    public class ByteTest
    {
        [Test]
        public void DefaultReaderConstructorTest()
        {
            ByteReader br = new ByteReader();
            br.Read<int>();
        }
    }
}
