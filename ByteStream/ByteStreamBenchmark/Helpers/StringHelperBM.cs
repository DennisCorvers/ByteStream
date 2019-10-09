using BenchmarkDotNet.Attributes;
using ByteStream;
using ByteStream.Mananged;
using ByteStream.Unmanaged;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ByteStreamBenchmark.Helpers
{
    public class StringHelperBM
    {
        private const int amount = 16;

        private readonly byte[] m_buf;
        private readonly ByteWriter bw;
        private readonly PtrWriter pw;

        public StringHelperBM()
        {
            int size = 4 * amount;
            m_buf = new byte[size];
            bw = new ByteWriter(m_buf);
            pw = new PtrWriter(Marshal.AllocHGlobal(size), size);
        }

        [Benchmark]
        public void PinAlways()
        {
            for (int i = 0; i < amount; i++)
            {
                bw.Write(i);
            }
        }

        [Benchmark]
        public void intptr()
        {
            for (int i = 0; i < amount; i++)
            {
                pw.Write(i);
            }
        }
    }
}
