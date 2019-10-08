﻿using ByteStream;
using ByteStreamTest.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamTest
{
    [TestFixture]
    public class BinaryHelperTest
    {
        private class IntPtrTest
        {
            private IntPtr m_buffer;

            [SetUp]
            public void Setup()
            {
                m_buffer = Marshal.AllocHGlobal(64);
            }
            [TearDown]
            public void Teardown()
            {
                if (m_buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(m_buffer);
            }

            [TestCase(108971523)]
            public void IntTest(int value)
            {
                int length = sizeof(int);

                for (int i = 0; i < 4 * 4; i += length)
                { BinaryHelper.Write(m_buffer, i, value); }

                for (int i = 0; i < 4 * length; i += length)
                { Assert.AreEqual(value, BinaryHelper.Read<int>(m_buffer, i)); }
            }

            [Test]
            public void BytesTest()
            {
                Assert.Catch(() => { BinaryHelper.WriteBytes(m_buffer, 0, null); });

                byte[] value = new byte[4] { 1, 2, 3, 5 };
                int length = 4;

                for (int i = 0; i < 4 * length; i += length)
                { BinaryHelper.WriteBytes(m_buffer, i, value); }

                for (int i = 0; i < 4 * length; i += length)
                { Assert.AreEqual(value, BinaryHelper.ReadBytes(m_buffer, i, 4)); }

                Assert.AreEqual(0, BinaryHelper.ReadBytes(m_buffer, 0, 0).Length);
            }

            [Test]
            public void StructTest()
            {
                BlittableStruct dat = new BlittableStruct() { ValOne = int.MaxValue, ValTwo = 123, ValThree = 321 };
                int length = 16;

                for (int i = 0; i < 4 * length; i += length)
                { BinaryHelper.Write(m_buffer, i, dat); }

                for (int i = 0; i < 4 * length; i += length)
                {
                    var other = BinaryHelper.Read<BlittableStruct>(m_buffer, i);
                    Assert.True(other.IsEqual(dat));
                }
            }

            [Test]
            public void ClearBytes()
            {
                Assert.Catch(() => { BinaryHelper.ClearBytes(m_buffer, 0, 0); });
                Assert.Catch(() => { BinaryHelper.ClearBytes(m_buffer, 0, 1025); });

                BinaryHelper.Write<byte>(m_buffer, 0, 1);
                BinaryHelper.ClearBytes(m_buffer, 0, 4);
                Assert.AreEqual(0, BinaryHelper.Read<byte>(m_buffer, 0));
            }
        }
        private class ByteArrTest
        {
            private byte[] m_buffer;

            [SetUp]
            public void Setup()
            {
                m_buffer = new byte[64];
            }
            [TearDown]
            public void Teardown()
            {

            }

            [Test]
            public void BytesTest()
            {
                byte[] value = new byte[4] { 1, 2, 3, 5 };
                int length = 4;

                for (int i = 0; i < 4 * length; i += length)
                { BinaryHelper.WriteBytes(m_buffer, i, value); }

                for (int i = 0; i < 4 * length; i += length)
                { Assert.AreEqual(value, BinaryHelper.ReadBytes(m_buffer, i, 4)); }

                Assert.AreEqual(0, BinaryHelper.ReadBytes(m_buffer, 0, 0).Length);
            }

            [TestCase(108971523)]
            public void IntTest(int value)
            {
                int length = sizeof(int);

                for (int i = 0; i < 4 * 4; i += length)
                { BinaryHelper.Write(m_buffer, i, value); }

                for (int i = 0; i < 4 * length; i += length)
                { Assert.AreEqual(value, BinaryHelper.Read<int>(m_buffer, i)); }
            }

            [Test]
            public void StructTest()
            {
                BlittableStruct dat = new BlittableStruct() { ValOne = int.MaxValue, ValTwo = 123, ValThree = 321 };
                int length = 16;

                for (int i = 0; i < 4 * length; i += length)
                { BinaryHelper.Write(m_buffer, i, dat); }

                for (int i = 0; i < 4 * length; i += length)
                {
                    var other = BinaryHelper.Read<BlittableStruct>(m_buffer, i);
                    Assert.True(other.IsEqual(dat));
                }
            }

            [Test]
            public void ClearBytes()
            {
                byte[] buf = new byte[4] { 1, 1, 1, 1 };
                Assert.Catch(() => { BinaryHelper.ClearBytes(buf, 0, 0); });

                BinaryHelper.ClearBytes(buf, 1, 2);
                Assert.AreEqual(new byte[] { 1, 0, 0, 1 }, buf);
            }
        }
    }
}
