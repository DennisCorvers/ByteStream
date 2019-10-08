using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ByteStreamBenchmark.Helpers;
using System;

namespace ByteStreamBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<StringHelperBM>();
            Console.ReadLine();
        }
    }
}
