using System;
using System.Threading;
using System.Threading.Tasks;
using A19.Concurrent.Cache;
using NUnit.Framework;

namespace A19.Concurrent.Test.Cache
{
    
    [TestFixture]
    public class CacheObjectTest
    {
        private object o1 = new Object();
        private object o2 = new Object();
        private int runCount = 0;

        [SetUp]
        public void SetUp()
        {
            this.runCount = 0;
        }
        
        
        [Test]
        public void LoadTest()
        {
            var cObject = new CacheObject<object>(this.LoadFunc, new TimeSpan(0, 1, 0));
            var r = cObject.Get();
            var p = cObject.Get();
            r.Wait();
            p.Wait();
            Assert.AreEqual(o1, r.Result);
            Assert.AreEqual(o1, r.Result);
        }

        [Test]
        public void ExpireTest()
        {
            var cObject = new CacheObject<object>(this.LoadFunc, new TimeSpan(0, 0, 0, 0, 150));
            var r = cObject.Get();
            r.Wait();
            Assert.AreEqual(o1, r.Result);
            Thread.Sleep(205);
            r = cObject.Get();
            r.Wait();
            Assert.AreEqual(o1, r.Result);
            Thread.Sleep(105);
            r = cObject.Get();
            Assert.AreEqual(o2, r.Result);
            
        }
        
        private Task<object> LoadFunc()
        {
            Thread.Sleep(95);
            if (runCount++ == 0)
            {
                return Task.FromResult(o1);
            }
            else
            {
                return Task.FromResult(o2);
            }
        }
    }
}