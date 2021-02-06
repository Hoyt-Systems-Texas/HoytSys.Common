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
                ("a", "c"),
                ("b", "c"),
                ("d", "c"),
            };
            var graph = ImmutableSparseGraph<string>.Create(edges);
            var matches = new List<string>(10);
            graph.Find("a", matches.Add);
            CollectionAssert.AreEquivalent(
                new [] {"b", "c"},
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
        }
    }
}