using System.Collections.Generic;
using HoytSys.DataStructures.Graph;
using NUnit.Framework;

namespace HoytSys.DataStructure.Test.Graph
{
    
    [TestFixture]
    public class SparseGraphTest
    {

        [Test]
        public void CreateTest()
        {
            var edges = new List<(string, string)>
            {
                ("a", "b"),
                ("a", "f"),
                ("a", "g"),
                ("a", "c"),
                ("b", "c"),
                ("d", "c"),
                ("f", "d"),
                ("f", "a"),
                ("f", "c"),
                ("f", "b"),
                ("g", "b"),
                ("g", "a"),
                ("g", "c"),
                ("g", "d"),
            };
            var graph = ImmutableSparseGraph<string>.Create(edges);
            var matches = new List<string>(10);
            graph.Find("a", matches.Add);
            CollectionAssert.AreEquivalent(
                new [] {"b", "c", "f", "g"},
                matches);

            matches = new List<string>(10);
            graph.Find("b", matches.Add);
            CollectionAssert.AreEquivalent(
                new [] {"c"},
                matches);
            matches = new List<string>(10);
            graph.Find("d", matches.Add);
            CollectionAssert.AreEquivalent(
                new [] {"c"},
                matches);
            matches = new List<string>(10);
            graph.Find("f", matches.Add);
            CollectionAssert.AreEquivalent(
                new [] {"d", "a", "c", "b"},
                matches);
        }

        [Test]
        public void BfsTest()
        {
            var edges = new List<(string, string)>
            {
                ("a", "b"),
                ("a", "d"),
                ("b", "c"),
                ("c", "f"),
                ("c", "a"),
                ("d", "c"),
                ("f", "d"),
                ("f", "a"),
                ("f", "c"),
                ("f", "b"),
                ("f", "g"),
                ("g", "b"),
                ("g", "a"),
                ("g", "c"),
                ("g", "d"),
            };
            var graph = ImmutableSparseGraph<string>.Create(edges);
            
            var result = graph.Bfs("a", "g");
            CollectionAssert.AreEqual(
                result,
                new List<string>{"a", "b", "c", "f", "g"});
        }
    }
}