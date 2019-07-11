using NUnit.Framework;

namespace A19.Concurrent.Test
{

    [TestFixture]
    public class FastSemaphoreTest
    {

        [Test]
        public void SimpleAcquireTest()
        {
            var semaphore = new FastSemaphore(2);
            Assert.IsTrue(semaphore.AcquireFailFast());
            Assert.IsTrue(semaphore.AcquireFailFast());
            Assert.IsFalse(semaphore.AcquireFailFast());
            semaphore.Release();
            Assert.IsTrue(semaphore.AcquireFailFast());
        }
    }
}