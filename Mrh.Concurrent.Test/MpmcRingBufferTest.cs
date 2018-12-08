using System.Threading;
using NUnit.Framework;

namespace Mrh.Concurrent.Test
{
    [TestFixture]
    public class MpmcRingBufferTest
    {
        [Test]
        public void AFillTest()
        {
            var buffer = new MpmcRingBuffer<ClassTest>(0x10);
            var value = new ClassTest();
            for (long i = 0; i < 0x10; i++)
            {
                Assert.IsTrue(buffer.Offer(value));
            }
            Assert.IsFalse(buffer.Offer(value));
        }

        [Test]
        public void TakeTest()
        {
            var buffer = new MpmcRingBuffer<ClassTest>(0x10);
            var value = new ClassTest();
            for (long i = 0; i < 0x10; i++)
            {
                Assert.IsTrue(buffer.Offer(value));
            }
            Assert.IsFalse(buffer.Offer(value));
            for (long i = 0; i < 0x10; i++)
            {
                Assert.IsTrue(buffer.TryPoll(out value));
            }
            Assert.IsFalse(buffer.TryPoll(out value));
        }

        [Test]
        public void ZBrutalTest()
        {
            var buffer = new MpmcRingBuffer<ClassTest>(0x100);
            var goThrough = 100;
            var threadCount = 5;
            var writeThreads = new Thread[threadCount];
            var readThreads = new Thread[threadCount];
            var value = new ClassTest();
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
                        buffer.TryPoll(out ClassTest ignore);
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

        public class ClassTest
        {
            public int Value { get; set; }
        }
    }
}