using HoytSys.DataStructures.Graph;
using NUnit.Framework;

namespace HoytSys.DataStructure.Test.Graph
{
    [TestFixture]
    public class UniformMatrixGraphTest
    {
        [Test]
        public void FindDependencyTest()
        {
            var graph = new UniformMatrixGraph(5);
            graph.MarkEdge(0,1);
            graph.MarkEdge(1,2);
            graph.MarkEdge(1,3);
            graph.MarkEdge(2,3);
            graph.MarkEdge(2, 4);

            var deps = graph.FindDependencies();
            Assert.IsNotEmpty(deps);
        }

        [Test]
        public void FindDependencyTestComplex()
        {
            var graph = new UniformMatrixGraph(6);
            
            graph.MarkEdge(0, 5);
            graph.MarkEdge(1, 2);
            graph.MarkEdge(3, 0);
            graph.MarkEdge(4, 2);

            var deps = graph.FindDependencies();
            Assert.IsNotEmpty(deps);
        }
    }
}