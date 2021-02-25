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
            BenchmarkRunner.Run<WriteCompare>();
            Console.ReadLine();
        }
    }
}
