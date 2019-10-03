using NUnit.Framework;
using ByteStream;
using System;
using System.Collections.Generic;
using System.Text;
using ByteStreamTest.Data;

namespace ByteStreamTest.BinaryHelperTests
{
    [TestFixture]
    public class BinaryHelperManagedTest
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

        [TestCase(true)]
        [TestCase(false)]
        public void BoolTest(bool value)
        {
            int length = sizeof(bool);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteBool(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadBool(m_buffer, i)); }
        }

        [TestCase(byte.MaxValue)]
        public void ByteTest(byte value)
        {
            int length = sizeof(byte);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteByte(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadByte(m_buffer, i)); }
        }

        [TestCase(sbyte.MinValue)]
        public void SByteTest(sbyte value)
        {
            int length = sizeof(sbyte);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteSByte(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadSByte(m_buffer, i)); }
        }

        [TestCase('漢')]
        public void CharTest(char value)
        {
            int length = sizeof(char);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteChar(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadChar(m_buffer, i)); }
        }

        [TestCase('a')]
        public void CharSingleTest(char value)
        {
            int length = sizeof(byte);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteCharSingle(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadCharSingle(m_buffer, i)); }
        }

        [TestCase(1232)]
        public void DecimalTest(decimal value)
        {
            int length = sizeof(decimal);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteDecimal(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadDecimal(m_buffer, i)); }
        }

        [TestCase(double.MinValue)]
        public void DoubleTest(double value)
        {
            int length = sizeof(double);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteDouble(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadDouble(m_buffer, i)); }
        }

        [TestCase(623.2161f)]
        public void SingleTest(float value)
        {
            int length = sizeof(float);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteSingle(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadSingle(m_buffer, i)); }
        }

        [TestCase(2134515125)]
        public void IntTest(int value)
        {
            int length = sizeof(int);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteInt(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadInt(m_buffer, i)); }
        }

        [TestCase(4102187965)]
        public void UIntTest(uint value)
        {
            int length = sizeof(uint);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteUInt(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadUInt(m_buffer, i)); }
        }

        [TestCase(long.MinValue)]
        public void LongTest(long value)
        {
            int length = sizeof(long);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteLong(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadLong(m_buffer, i)); }
        }

        [TestCase(ulong.MaxValue)]
        public void ULongTest(ulong value)
        {
            int length = sizeof(ulong);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteULong(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadULong(m_buffer, i)); }
        }

        [TestCase(-13021)]
        public void ShortTest(short value)
        {
            int length = sizeof(short);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteShort(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadShort(m_buffer, i)); }
        }

        [TestCase((ushort)52012)]
        public void UShortTest(ushort value)
        {
            int length = sizeof(ushort);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteUShort(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadUShort(m_buffer, i)); }
        }



        [TestCase("手機瀏覽")]
        public void DBCSTest(string value)
        {
            int length = value.Length * sizeof(char);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteDBCS(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadDBCS(m_buffer, i, value.Length)); }
        }

        [TestCase("Test.")]
        public void ASCIITest(string value)
        {
            int length = value.Length;

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteASCII(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadASCII(m_buffer, i, value.Length)); }
        }

        [TestCase("手機瀏覽")]
        public void UTF8Test(string value)
        {
            int length = Encoding.UTF8.GetByteCount(value);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteUTF8(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadUTF8(m_buffer, i, length)); }
        }

        [TestCase("手機瀏覽")]
        public void UTF16Test(string value)
        {
            int length = Encoding.Unicode.GetByteCount(value);

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteUTF16(m_buffer, i, value); }

            for (int i = 0; i < 4 * length; i += length)
            { Assert.AreEqual(value, BinaryHelper.ReadUTF16(m_buffer, i, length)); }
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
        }

        [Test]
        public void StructTest()
        {
            BlittableStruct dat = new BlittableStruct() { ValOne = 15423, ValTwo = true, ValThree = 9170 };
            int length = 11;

            for (int i = 0; i < 4 * length; i += length)
            { BinaryHelper.WriteValue(m_buffer, i, dat); }

            for (int i = 0; i < 4 * length; i += length)
            {
                var other = BinaryHelper.ReadValue<BlittableStruct>(m_buffer, i);
                Assert.True(other.IsEqual(dat));
            }
        }
    }
}
