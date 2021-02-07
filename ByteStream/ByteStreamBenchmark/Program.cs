using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ByteStreamBenchmark.Comparison;
using System;
using System.Runtime.InteropServices;

namespace ByteStreamBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Temp>();
            Console.ReadLine();
        }
    }

    public unsafe class Temp
    {
        byte[] arr;
        IntPtr ptr;

        [GlobalSetup]
        public void Setup()
        {
            arr = new byte[1024];
            ptr = Marshal.AllocHGlobal(1024);
        }
        [GlobalCleanup]
        public void Cleanup()
        {
            arr = null;
            Marshal.FreeHGlobal(ptr);
        }

        [Benchmark]
        public void Old()
        {
            int length = 1023;

            byte* zeros = stackalloc byte[length];
            Buffer.MemoryCopy(zeros, (void*)(ptr + 0), length, length);
        }

        [Benchmark]
        public void New()
        {
            int length = 1023;

            long c = length >> 3; // longs

            int i = 0;
            for (; i < c; i++)
                *((ulong*)ptr + i) = 0;

            i = i << 3;
            for (; i < length; i++)
                *((byte*)ptr + i) = 0;
        }

        public unsafe static void CopyMarshal(IntPtr source, int sourceIndex, byte[] destination, int destinationIndex, int length)
        {
            Marshal.Copy(source + sourceIndex, destination, destinationIndex, length);
        }

        public unsafe static void CopyUnsafe(IntPtr source, int sourceIndex, byte[] destination, int destinationIndex, int length)
        {
            fixed (byte* dst = &destination[destinationIndex])
            {
                Buffer.MemoryCopy((byte*)source + sourceIndex, dst, length, length);
            }
        }
    }
}
