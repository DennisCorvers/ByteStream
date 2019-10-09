using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ByteStreamBenchmark.Comparison;
using System;

namespace ByteStreamBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ReadCompare>();
            Console.ReadLine();
        }
    }
}
