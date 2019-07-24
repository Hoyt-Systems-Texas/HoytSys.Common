using System.Collections.Generic;
using A19.DataStructures.Graph;

namespace A19.Database.Diff
{
    public class BaseNode<TNew, TDb, TKey, TUserId> :
        IDbUpdateNode,
        INodeChildCollection<TNew, TDb, TKey, TUserId>,
        IBuildDependencyGraph
        where TDb : AbstractDatabaseRecord<TKey>
    {
        private readonly List<IUpdateManyToOne<TNew, TDb, TKey, TUserId>> manyToOnes =
            new List<IUpdateManyToOne<TNew, TDb, TKey, TUserId>>(2);

        private readonly List<IUpdateManyToMany<TNew, TDb, TKey, TUserId>> manyToMany =
            new List<IUpdateManyToMany<TNew, TDb, TKey, TUserId>>(2);

        private readonly int nodeId;

        public BaseNode(int nodeId)
        {
            this.nodeId = nodeId;
        }

        public IEnumerable<IUpdateManyToOne<TNew, TDb, TKey, TUserId>> ManyToOnes => manyToOnes;

        public IEnumerable<IUpdateManyToMany<TNew, TDb, TKey, TUserId>> ManyToMany => manyToMany;

        public void AddManyToOne(IUpdateManyToOne<TNew, TDb, TKey, TUserId> manyToOne)
        {
            this.manyToOnes.Add(manyToOne);
        }

        public void AddManyToMany(IUpdateManyToMany<TNew, TDb, TKey, TUserId> manyToMany)
        {
            this.manyToMany.Add(manyToMany);
        }

        public void BuildDependencyGraph(UniformMatrixGraph graph)
        {
            foreach (var node in this.manyToOnes)
            {
                graph.MarkEdge(this.nodeId, node.NodeId);
                node.BuildDependencyGraph(graph);
            }

            foreach (var node in manyToMany)
            {
                graph.MarkEdge(node.NodeId, this.nodeId);
                node.BuildDependencyGraph(graph);
            }
        }

        public int NodeId => this.nodeId;

        protected bool RunManyToOneUpdate(
            TNew newValue,
            TDb value, 
            Dictionary<int, IUpdateRecords<TUserId>> updateValues)
        {
            var changed = false;
            foreach (var manyToOne in this.manyToOnes)
            {
                changed = manyToOne.Update(newValue, value, updateValues)
                    || changed;
            }

            return changed;
        }

        protected bool RunManyToManyUpdate(
            TNew newValue,
            TDb value,
            Dictionary<int, IUpdateRecords<TUserId>> updateValues)
        {
            var changed = false;
            foreach (var many in this.manyToMany)
            {
                changed = many.Update(
                    newValue,
                    value,
                    updateValues) || changed;
            }
            return changed;
        }
    }
}