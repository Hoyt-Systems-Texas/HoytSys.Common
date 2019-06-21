using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Mrh.Concurrent.Test
{
    [TestFixture]
    public class ConcurrentBagTest
    {
        [Test]
        public void AddTest()
        {
            var bag = new ConcurrentArrayBag<TestClass>(2);

            for (var i = 0; i < 100; i++)
            {
                bag.Add(new TestClass(i));
            }

            var hashSet = new HashSet<int>(bag.Select(val => val.MyInt));

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(hashSet.Contains(i));
            }
        }

        [Test]
        public void ConcurrentTest()
        {
            var bag = new ConcurrentArrayBag<TestClass>(10000);
            var threadCount = 20;
            var iterations = 100000;
            var threads = new Thread[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (var j = 0; j < iterations; j++)
                    {
                        if (i % threadCount == 0)
                        {
                            bag.Add(new TestClass(j));
                        }
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
            
            var hashSet = new HashSet<int>(bag.Select(val => val.MyInt));

            for (int i = 0; i < iterations; i++)
            {
                Assert.IsTrue(hashSet.Contains(i));
            }
        }

        [Test]
        public void TestRemove()
        {
            
            var bag = new ConcurrentArrayBag<TestClass>(100);
            var threadCount = 8;
            var iterations = 10000;
            var threads = new Thread[threadCount];
            var removed = new HashSet<int>[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                var pos = i;
                removed[i] = new HashSet<int>(100);
                threads[i] = new Thread(() =>
                {
                    var rand = new Random();
                    for (var j = 0; j < iterations; j++)
                    {
                        if (pos % threadCount == 0)
                        {
                            bag.Add(new TestClass(j));
                        }

                        if (rand.NextDouble() < 0.05)
                        {
                            var removeValue = rand.Next(0, j);
                            bag.Remove(v => v.MyInt == removeValue);
                            removed[pos].Add(removeValue);
                        }
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
            
            var hashSet = new HashSet<int>(bag.Select(val => val.MyInt));
            var removedSet = new HashSet<int>();
            foreach (var set in removed)
            {
                foreach (var i in set)
                {
                    removedSet.Add(i);
                }
            }

            for (int i = 0; i < iterations; i++)
            {
                Assert.IsTrue(removedSet.Contains(i) || hashSet.Contains(i));
            }
        }

        public class TestClass
        {
            public readonly int MyInt;

            public TestClass(int i)
            {
                this.MyInt = i;
            }
        }
    }
}