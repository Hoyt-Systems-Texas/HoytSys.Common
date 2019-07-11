using System;
using System.Threading;
using NUnit.Framework;

namespace A19.Concurrent.Test
{
    
    [TestFixture]
    public class SimpleRetryTest
    {

        [Test]
        public void RunTest()
        {
            var retry = new SimpleRetry(
                100,
                (ex) =>
                {
                    
                });
            retry.Start();
            var success = 0;
            retry.Retry(
                new TimeSpan(0, 0, 0, 0, 5),
                () => { Interlocked.Exchange(ref success, 1); });
            Thread.Sleep(15);
            Assert.AreEqual(1, Volatile.Read(ref success));
        }
        
    }
}