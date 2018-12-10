using System;
using System.Threading;
using Mrh.Concurrent;

namespace Mrr.Concurrent.Console.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var buffer = new MpmcRingBuffer<MyTestClass>(0x10000);
            var goThrough = 10_000_000;
            var threadCount = 12;
            var writeThreads = new Thread[threadCount];
            var readThreads = new Thread[threadCount];
            var value = new MyTestClass();
            for (var i = 0; i < threadCount; i++)
            {
                writeThreads[i] = new Thread((obj) =>
                {
                    for (var j = 0; j < goThrough; j++)
                    {
                        buffer.Offer(value);
                    }
                });
                readThreads[i] = new Thread((obj) =>
                {
                    for (var j = 0; j < goThrough; j++)
                    {
                        buffer.TryPoll(out MyTestClass ignore);
                    }
                });
            }

            for (var i = 0; i < threadCount; i++)
            {
                writeThreads[i].Start();
                readThreads[i].Start();
            }

            for (var i = 0; i < threadCount; i++)
            {
                writeThreads[i].Join();
                readThreads[i].Join();
            }

        }

        public class MyTestClass
        {
            
        }
    }
}