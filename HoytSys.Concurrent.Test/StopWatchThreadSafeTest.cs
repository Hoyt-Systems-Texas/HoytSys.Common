using System.Threading;
using NUnit.Framework;

namespace A19.Concurrent.Test
{
    [TestFixture]
    public class StopWatchThreadSafeTest
    {

        [Test]
        public void SimpleTest()
        {
            var watch = new StopWatchThreadSafe();
            Thread.Sleep(5);
            Assert.AreEqual(StopWatchThreadSafe.MillsToFrequency(5), watch.Elapsed(), 500000);
        }
    }
}