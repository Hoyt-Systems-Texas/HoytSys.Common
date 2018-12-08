using NUnit.Framework;

namespace Mrh.Concurrent.Test
{
    [TestFixture]
    public class ConcurrentRingBufferTest
    {

        [Test]
        public void AWriteTest()
        {
            var buffer = new ConcurrentRingBuffer<MyComplexValue>(0x1000);
            var value = new MyComplexValue();
            for (long i = 0; i < 0x1000; i++)
            {
                Assert.IsTrue(buffer.Set(i, value));
            }
            for (long i = 0; i < 0x1000; i++)
            {
                Assert.IsTrue(buffer.TryGet(i, out value));
            }
            for (long i = 0; i < 0x1000; i++)
            {
                Assert.IsTrue(buffer.Clear(i));
            }
        }

        [Test]
        public void SpeedRunTest()
        {
            long size = 0x1000000;
            var buffer = new ConcurrentRingBuffer<MyComplexValue>((uint) size);
            var value = new MyComplexValue();
            for (long i = 0; i < size; i++)
            {
                buffer.Set(i, value);
            }

            for (long i = 0; i < size; i++)
            {
                buffer.TryGet(i, out value);
            }

            for (long i = 0; i < size; i++)
            {
                buffer.Clear(i);
            }
        }

        private class MyComplexValue
        {
            public int Value { get; set; }
        }
    }
}