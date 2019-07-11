using System.Threading;
using System.Threading.Tasks;
using A19.Concurrent.Agent;
using NUnit.Framework;

namespace A19.Concurrent.Test.Agent
{
    [TestFixture]
    public class AgentMonadTest
    {

        [Test]
        public void BindTest()
        {
            bool ran = false;
            var context = new ContextTest
            {
                Id = 1
            };
            var runner = new AgentLocalRunner<ContextTest>(
                1,
                0x10,
                context);
            var monad = runner.To(1);
            monad.Bind((ctx, _) =>
            {
                ran = true; 
                return Task.FromResult(runner.To(2));
            }).Wait();
            Assert.IsTrue(ran);
        }

        [Test]
        public void SelectTest()
        {
            bool ran = false;
            var context = new ContextTest
            {
                Id = 1
            };
            var runner = new AgentLocalRunner<ContextTest>(
                1,
                0x10,
                context);
            var monad = runner.To(1);
            monad.Select((ctx, _) =>
            {
                ran = true; 
                return Task.FromResult(runner.To(2));
            }).Wait();
            Assert.IsTrue(ran);
        }

        [Test]
        public void MultiThreadTest()
        {
            var threadCount = 6;
            var spinCount = 1_000_000;
            var total = 0;
            var runner = new AgentLocalRunner<ContextTest>(
                1,
                0x1000,
                new ContextTest());
            var monad = runner.To(1);
            var threads = new Thread[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread((obj) =>
                {
                    for (var j = 0; j < spinCount; j++)
                    {
                        monad.Select((val, _) =>
                        {
                            Interlocked.Increment(ref total);
                            return Task.FromResult(0);
                        }).Wait();
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            Assert.AreEqual(total, spinCount * threadCount);
        }
        
        private class ContextTest : IAgentContext
        {
            public long Id { get; set; }
        }
    }
}