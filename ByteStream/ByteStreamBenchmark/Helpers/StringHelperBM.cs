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
        private readonly byte[] m_buf;
        private readonly ByteWriter bw;
        private  ByteWriterFast bwf;
        private readonly PtrWriter pw;

        public StringHelperBM()
        {
            m_buf = new byte[64];
            bw = new ByteWriter(m_buf);
            pw = new PtrWriter(Marshal.AllocHGlobal(64), 64);
            bwf = new ByteWriterFast(new byte[64]);
        }

        [IterationSetup]
        public void Test()
        {
            bwf = new ByteWriterFast(new byte[64]);
        }

        //[Benchmark]
        public void PinAlways()
        {
            for (int i = 0; i < 16; i++)
            {
                bw.Write(i);
            }
        }

        [Benchmark]
        public void PinOnce()
        {
            bwf.Pin();
            for (int i = 0; i < 16; i++)
            {
                bwf.Write(i);
            }
            bwf.UnPin();
            bwf.Dispose();

        }

        //[Benchmark]
        public void intptr()
        {
            for (int i = 0; i < 16; i++)
            {
                pw.Write(i);
            }
        }

    }
}
