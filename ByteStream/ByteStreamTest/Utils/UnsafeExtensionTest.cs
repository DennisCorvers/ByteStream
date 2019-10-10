using NUnit.Framework;
using ByteStream.Utils.Unsafe;
using ByteStream.Mananged;
using ByteStream.Unmanaged;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ByteStream;

namespace ByteStreamTest.Utils
{
    [TestFixture]
    public unsafe class UnsafeExtensionTest
    {
        [Test]
        public void WritePtrByteWriterTest()
        {
            var bw = new ByteWriter(new byte[32]);
            byte* ptr = stackalloc byte[4] { 1, 2, 3, 4 };
            bw.WritePtr(ptr, 4);

            Assert.AreEqual(4, bw.Offset);
            for (int i = 0; i < 4; i++)
            { Assert.AreEqual(i + 1, bw.Buffer[i]); }
        }

        [Test]
        public void WritePtrPtrWriterTest()
        {
            var pw = new PtrWriter(Alloc(32), 32);
            byte* ptr = stackalloc byte[4] { 1, 2, 3, 4 };
            pw.WritePtr(ptr, 4);

            Assert.AreEqual(4, pw.Offset);
            for (int i = 0; i < 4; i++)
            { BinaryHelper.Read<byte>(pw.Buffer, i); }

            Free(pw.Buffer);
        }

        [TestCase(123456789)]
        public void ReadPtrByteReaderTest(int value)
        {
            byte[] dat = new byte[4];
            BinaryHelper.Write(dat, 0, value);

            ByteReader br = new ByteReader(dat);
            IntPtr dest = Alloc(4);
            br.ReadPtr(dest.ToPointer(), 4);

            Assert.AreEqual(4, br.Offset);
            Assert.AreEqual(value, BinaryHelper.Read<int>(dest, 0));
            Free(dest);
        }

        [TestCase(987654321)]
        public void ReadPtrPtrReaderTest(int value)
        {
            IntPtr ptr = Alloc(4);
            BinaryHelper.Write(ptr, 0, value);

            PtrReader pr = new PtrReader(ptr, 4);
            IntPtr dest = Alloc(4);
            pr.ReadPtr(dest.ToPointer(), 4);

            Assert.AreEqual(4, pr.Offset);
            Assert.AreEqual(value, BinaryHelper.Read<int>(dest, 0));

            Free(ptr);
            Free(dest);
        }

        [TestCase(12.5d, 51)]
        public void PtrWriterAsPointerTest(double vald, long vall)
        {
            PtrWriter pw = new PtrWriter(Alloc(16), 16);
            var asPointer = pw.AsPointer();

            asPointer->Write(vald);
            asPointer->Write(vall);

            Assert.AreEqual(16, asPointer->Offset);
            Assert.AreEqual(16, asPointer->Length);
            Assert.AreEqual(vald, BinaryHelper.Read<double>(pw.Buffer, 0));
            Assert.AreEqual(vall, BinaryHelper.Read<long>(pw.Buffer, 8));

            Free(asPointer->Buffer);
            Free(asPointer);
        }

        [TestCase(50.1d, 12)]
        public void PtrReaderAsPointerTest(double vald, long vall)
        {
            IntPtr data = Alloc(16);
            BinaryHelper.Write(data, 0, vald);
            BinaryHelper.Write(data, 8, vall);

            PtrReader pr = new PtrReader(data, 16);
            var asPointer = pr.AsPointer();

            Assert.AreEqual(vald, asPointer->Read<double>());
            Assert.AreEqual(vall, asPointer->Read<long>());
            Assert.AreEqual(16, asPointer->Offset);
            Assert.AreEqual(16, asPointer->Length);

            Free(asPointer->Buffer);
            Free(asPointer);
        }

        private static void Free(void* mem)
        { Marshal.FreeHGlobal((IntPtr)mem); }
        private static void Free(IntPtr mem)
        { Marshal.FreeHGlobal(mem); }
        private static IntPtr Alloc(int size)
        { return Marshal.AllocHGlobal(size); }
    }
}
