using NUnit.Framework;
using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStreamTest.Utils
{
    [TestFixture]
    public class ExtensionsTest
    {
        [TestCase(16)]
        [TestCase(9)]
        public void NextPowerOfTwo(int value)
        {
            Assert.AreEqual(16, value.NextPowerOfTwo());
        }
    }
}
