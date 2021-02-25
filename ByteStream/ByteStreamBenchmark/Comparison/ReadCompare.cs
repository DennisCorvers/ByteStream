using BenchmarkDotNet.Attributes;
using ByteStream;
using ByteStream.Mananged;
using ByteStream.Unmanaged;
using ByteStreamBenchmark.Helper;
using System;
using System.Runtime.InteropServices;

namespace ByteStreamBenchmark.Comparison
{
    [MemoryDiagnoser]
    public class ReadCompare
    {
        private const int AMOUNT = 1024;
        private readonly byte[] m_byteBuf;
        private readonly IntPtr m_ptrBuf;
        private readonly ManagedStream managedStream;
        private readonly UnmanagedStream unmanagedStream;

        public ReadCompare()
        {
            managedStream = new ManagedStream();
            unmanagedStream = new UnmanagedStream();
            int size = sizeof(int) * AMOUNT;
            m_byteBuf = new byte[size];
            m_ptrBuf = Marshal.AllocHGlobal(size);

            for (int i = 0; i < size; i += 4)
            {
                int num = i / 4;
                BinaryHelper.Write(m_byteBuf, i, num);
                BinaryHelper.Write(m_ptrBuf, i, num);
            }
        }
        ~ReadCompare()
        {
            Marshal.FreeHGlobal(m_ptrBuf);
        }

        [Benchmark]
        public void ByteRead()
        {
            ByteReader reader = new ByteReader(m_byteBuf);
            for (int i = 0; i < AMOUNT; i++)
            { int result = reader.Read<int>(); }
        }

        [Benchmark]
        public void PtrRead()
        {
            PtrReader reader = new PtrReader(m_ptrBuf, AMOUNT * sizeof(int));
            for (int i = 0; i < AMOUNT; i++)
            { int result = reader.Read<int>(); }
        }

        [Benchmark]
        public void ManagedStream()
        {
            managedStream.ResetRead(m_byteBuf);

            for (int i = 0; i < AMOUNT; i++)
            {
                int result = managedStream.Read<int>();
            }
        }

        [Benchmark]
        public void UnmanagedStream()
        {
            unmanagedStream.ResetRead(m_ptrBuf, AMOUNT * sizeof(int));

            for (int i = 0; i < AMOUNT; i++)
            {
                int result = unmanagedStream.Read<int>();
            }
        }

        [Benchmark(Baseline = true)]
        public void ArraySegment()
        {
            ArraySegment<byte> writer = new ArraySegment<byte>(m_byteBuf);
            int offset = 0;

            for (int i = 0; i < AMOUNT; i++)
            {
                int result = 0;
                for (int j = 0; j < sizeof(int); j++)
                {
                    result |= writer[offset] << j * 8;
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
                int result = BitConverter.ToInt32(m_byteBuf, offset);
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
                for (int j = 0; j < sizeof(int); j++)
                {
                    converter[j] = m_byteBuf[offset];
                    offset++;
                }
                int result = converter.int32;
            }
        }
    }
}
