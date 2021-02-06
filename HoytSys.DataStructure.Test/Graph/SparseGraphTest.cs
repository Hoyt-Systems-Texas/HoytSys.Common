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
                ("b", "c"),
                ("a", "c"),
                ("d", "c"),
            };
            var graph = ImmutableSparseGraph<string>.Create(edges);
            
        }
    }
}