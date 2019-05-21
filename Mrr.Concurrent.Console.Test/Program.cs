using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Mrh.Concurrent;

namespace Mrr.Concurrent.Console.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var buffer = new MpmcRingBuffer<MyTestClass>(0x4000000);
            var goThrough = 100_000_000;
            var threadCount = 2;
            var writeThreads = new Thread[threadCount];
            var readThreads = new Thread[threadCount];
            var value = new MyTestClass();
            var stopWatch = new Stopwatch();
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
            stopWatch.Start();
            for (var i = 0; i < threadCount; i++)
            {
                writeThreads[i].Join();
                readThreads[i].Join();
            }

            var time = stopWatch.ElapsedMilliseconds;
            File.WriteAllText(@"D:\Ran.txt", $"Ran in {time}");

        }

        public class MyTestClass
        {
            
        }
    }
}