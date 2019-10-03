using NUnit.Framework;
using ByteStream.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace ByteStreamTest.Utils
{
    [TestFixture]
    public class ArrayExtensionTest
    {
        private int[] m_source;
        private int[] m_dest;

        [SetUp]
        public void Init()
        {
            m_source = new int[5] { 1, 2, 3, 4, 5 };
            m_dest = new int[3];
        }

        [Test]
        public void CopyToUnsafe()
        {
            int srcIndex = 1;
            int dstIndex = 1;
            int len = 2;

            m_source.CopyToUnsafe(srcIndex, m_dest, dstIndex, len);
            for (int i = 0; i < len; i++)
            { Assert.AreEqual(m_dest[dstIndex + i], m_source[srcIndex + i]); }

        }

        [Test]
        public void CloneUnsafe()
        {
            int[] cpy = m_source.CloneUnsafe();
            for (int i = 0; i < m_source.Length; i++)
            { Assert.AreEqual(cpy[i], m_source[i]); }
        }

        [Test]
        public void ResizeUnsafe()
        {
            int len = 3;
            ArrayExtensions.ResizeUnsafe(ref m_source, len);

            Assert.AreEqual(m_source.Length, len);
        }
    }
}
