using System;
using System.Collections.Generic;
using HoytSys.DataStructures;
using NUnit.Framework;

namespace HoytSys.DataStructure.Test
{
    [TestFixture]
    public class BinaryHeapTest
    {
        [Test]
        public void AddTest()
        {
            var rand = new Random(0);
            var num = 10000;
            var heap = new BinaryHeap<int>(16, new IntComaparer());
            var values = new List<int>(100);
            for (var i = 0; i < 100; i++)
            {
                var newNum = rand.Next(100);
                heap.Put(newNum);
                values.Add(newNum);
            }

            var prev = -1;
            Assert.IsTrue(heap.VerifyHeap(0));
            values.Sort(new IntComaparer());
            for (var i = 0; i < 100; i++)
            {
                int c = 0;
                Assert.IsTrue(heap.Take(out c));
                Assert.LessOrEqual(prev, c);
                Assert.AreEqual(values[i], c);
                prev = c;
            }
        }

        [Test]
        public void RandomTest()
        {
            var num = 1000;
            for (var i = 0; i < num; i++)
            {
                var rand = new Random(0);
                var heap = new BinaryHeap<int>(16, new IntComaparer());
                var values = new List<int>(100);
                for (var j = 0; j < 100; j++)
                {
                    var newNum = rand.Next(100);
                    heap.Put(newNum);
                    values.Add(newNum);
                }

                var prev = -1;
                Assert.IsTrue(heap.VerifyHeap(0));
                values.Sort(new IntComaparer());
                for (var j = 0; j < 100; j++)
                {
                    int c = 0;
                    Assert.IsTrue(heap.Take(out c));
                    Assert.LessOrEqual(prev, c);
                    Assert.AreEqual(values[j], c);
                    prev = c;
                }
            }
        }

        private class IntComaparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y);
            }
        }
    }
}