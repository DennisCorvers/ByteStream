using BenchmarkDotNet.Attributes;
using ByteStream.Mananged;
using ByteStream.Unmanaged;
using ByteStreamBenchmark.Helper;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamBenchmark.Comparison
{
    public class WriteCompare
    {
        private const int AMOUNT = 1024;
        private readonly byte[] m_byteBuf;
        private readonly IntPtr m_ptrBuf;

        public WriteCompare()
        {
            int size = sizeof(int) * AMOUNT;
            m_byteBuf = new byte[size];
            m_ptrBuf = Marshal.AllocHGlobal(size);
        }
        ~WriteCompare()
        {
            Marshal.FreeHGlobal(m_ptrBuf);
        }

        [Benchmark]
        public void ByteWrite()
        {
            ByteWriter writer = new ByteWriter(m_byteBuf);
            for (int i = 0; i < AMOUNT; i++)
            { writer.Write(i); }
        }

        [Benchmark]
        public void PtrWrite()
        {
            PtrWriter writer = new PtrWriter(m_ptrBuf, AMOUNT*sizeof(int));
            for (int i = 0; i < AMOUNT; i++)
            { writer.Write(i); }
        }

        [Benchmark(Baseline = true)]
        public void ArraySegment()
        {
            ArraySegment<byte> writer = new ArraySegment<byte>(m_byteBuf);
            int offset = 0;

            for (int i = 0; i < AMOUNT; i++)
            {
                for (int j = 0; j < sizeof(int); j++)
                {
                    writer[offset] = (byte)(i << j * 8);
                    offset++;
                }
            }
        }

        [Benchmark]
        public void BitConvert()
        {
            int offset = 0;
            for (int i = 0; i < AMOUNT; i++)
            {
                var dat = BitConverter.GetBytes(i);
                Array.Copy(dat, 0, m_byteBuf, offset, dat.Length);
                offset += sizeof(int);
            }
        }

        [Benchmark]
        public void Union()
        {
            int offset = 0;
            for (int i = 0; i < AMOUNT; i++)
            {
                ByteConverter converter = new ByteConverter();
                converter.int32 = i;
                for (int j = 0; j < sizeof(int); j++)
                {
                    m_byteBuf[offset] = converter[j];
                    offset++;
                }
            }
        }
    }
}
