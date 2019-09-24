using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace A19.Database.Diff
{
    /// <summary>
    ///     The diff DSL library for generating a comparision. 
    /// </summary>
    /// <typeparam name="TNew"></typeparam>
    /// <typeparam name="TDb"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TUserId"></typeparam>
    public class DiffDsl<TNew, TDb, TKey, TUserId> where TDb : AbstractDatabaseRecord<TKey, TNew>, new()
    {
        private readonly IDiffRepository<TUserId, TKey, TDb, TNew> diffRepository;
        private readonly List<IUpdateValue<TNew, TDb, TKey>> updateValues = new List<IUpdateValue<TNew, TDb, TKey>>(10);
        private readonly List<IUpdateManyToOneNode> manyToOneNodes = new List<IUpdateManyToOneNode>(2);
        private readonly List<IUpdateManyToManyNode> manyToManyNodes = new List<IUpdateManyToManyNode>(2);
        private readonly bool immutable;

        public DiffDsl(
            IDiffRepository<TUserId, TKey, TDb, TNew> diffRepository,
            bool immutable)
        {
            this.diffRepository = diffRepository;
            this.immutable = immutable;
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add<TProperty>(
            Func<TNew, TProperty> newValues,
            Expression<Func<TDb, TProperty>> dbValue,
            Func<TProperty, TProperty, bool> func)
        {
            this.updateValues.Add(
                new FieldUpdateValue<TNew, TDb, TProperty, TKey, TUserId>(
                    newValues,
                    dbValue,
                    func));
            return this;
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> AddOne<TNewProp, TDbProp, TChildKey>(
            Func<TNew, TNewProp> newProp,
            Expression<Func<TDb, TDbProp>> dbValue,
            DiffDsl<TNewProp, TDbProp, TChildKey, TUserId> diffDsl)
            where TDbProp : AbstractDatabaseRecord<TChildKey, TNewProp>, new()
            where TNewProp: class
        {
            this.manyToOneNodes.Add(new ManyToOneNode<TNewProp,TDbProp,TChildKey>(
                newProp,
                dbValue,
                diffDsl));
            return this;
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> AddMany<TNewProp, TDbProp, TChildKey>(
            Func<TNewProp, TChildKey> newPropKey,
            Func<TNew, IEnumerable<TNewProp>> newProp,
            Func<TDb, List<TDbProp>> dbValue,
            Action<TDbProp, TDb> setParent,
            DiffDsl<TNewProp, TDbProp, TChildKey, TUserId> diffDsl)
            where TDbProp : AbstractDatabaseRecord<TChildKey, TNewProp>, new()
            where TNewProp: class
        {
            this.manyToManyNodes.Add(
                new ManyToManyNode<TNewProp,TDbProp,TChildKey>(
                    newPropKey,
                    newProp,
                    dbValue,
                    setParent,
                    diffDsl));
            return this;
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, int> newValues,
            Expression<Func<TDb, int>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, int?> newValues,
            Expression<Func<TDb, int?>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, DateTime> newValues,
            Expression<Func<TDb, DateTime>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, DateTime?> newValues,
            Expression<Func<TDb, DateTime?>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, long> newValues,
            Expression<Func<TDb, long>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, long?> newValues,
            Expression<Func<TDb, long?>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, string> newValues,
            Expression<Func<TDb, string>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add<TV>(
            Func<TNew, TV> newValue,
            Expression<Func<TDb, TV>> dbValue)
            where TV:struct
        {
            return this.Add(
                newValue,
                dbValue,
                (s1, s2) => s1.Equals(s2));
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, bool> newValues,
            Expression<Func<TDb, bool>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, decimal> newValues,
            Expression<Func<TDb, decimal>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, decimal?> newValues,
            Expression<Func<TDb, decimal?>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, Guid> newValues,
            Expression<Func<TDb, Guid>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, Guid?> newValues,
            Expression<Func<TDb, Guid?>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, short> newValues,
            Expression<Func<TDb, short>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, short?> newValues,
            Expression<Func<TDb, short?>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }

        /// <summary>
        ///     Called on the root to build the root node.
        /// </summary>
        /// <returns>Returns the root node.</returns>
        public (RootNode<TNew, TDb, TKey, TUserId>, int) Build()
        {
            var id = new IdPointer();
            var node = new RootNode<TNew, TDb, TKey, TUserId>(
                id.Id++,
                this.immutable,
                this.diffRepository,
                this.updateValues);
            this.Children(
                id,
                node);
            return (node, id.Id);
        }

        /// <summary>
        ///     Called to build the child nodes.
        /// </summary>
        /// <param name="idPointer">The id pointer.</param>
        /// <param name="baseNode">The base node.</param>
        public void Children(
            IdPointer idPointer,
            BaseNode<TNew, TDb, TKey, TUserId> baseNode)
        {
            foreach (var oneNode in manyToOneNodes)
            {
                baseNode.AddManyToOne(oneNode.Build(idPointer));
            }

            foreach (var manyNode in manyToManyNodes)
            {
                baseNode.AddManyToMany(manyNode.Build(idPointer));
            }
        }

        public interface IUpdateManyToOneNode
        {
            IUpdateManyToOne<TNew, TDb, TKey, TUserId> Build(IdPointer id);
        }

        public interface IUpdateManyToManyNode
        {
            IUpdateManyToMany<TNew, TDb, TKey, TUserId> Build(IdPointer id);
        }

        public class ManyToOneNode<TNewProp, TDbProp, TChildKey>
            : IUpdateManyToOneNode
            where TDbProp : AbstractDatabaseRecord<TChildKey, TNewProp>, new()
            where TNewProp : class
        {
            public readonly Func<TNew, TNewProp> NewProp;
            public readonly Expression<Func<TDb, TDbProp>> dbValue;
            public DiffDsl<TNewProp, TDbProp, TChildKey, TUserId> DiffDsl;

            public ManyToOneNode(
                Func<TNew, TNewProp> newProp,
                Expression<Func<TDb, TDbProp>> dbValue,
                DiffDsl<TNewProp, TDbProp, TChildKey, TUserId> DiffDsl)
            {
                this.NewProp = newProp;
                this.dbValue = dbValue;
                this.DiffDsl = DiffDsl;
            }

            public IUpdateManyToOne<TNew, TDb, TKey, TUserId> Build(IdPointer id)
            {
                var updateManyToOne = new UpdateManyToOne<TNew, TDb, TNewProp, TDbProp, TKey, TChildKey, TUserId>(
                    id.Id++,
                    DiffDsl.immutable,
                    NewProp,
                    dbValue,
                    DiffDsl.updateValues,
                    DiffDsl.diffRepository);
                DiffDsl.Children(
                    id,
                    updateManyToOne);
                return updateManyToOne;
            }
        }

        public class ManyToManyNode<TNewProp, TDbProp, TChildKey> : IUpdateManyToManyNode
            where TDbProp : AbstractDatabaseRecord<TChildKey, TNewProp>, new()
            where TNewProp : class
        {
            public readonly Func<TNewProp, TChildKey> NewPropKey;
            public readonly Func<TNew, IEnumerable<TNewProp>> NewProp;
            public readonly Func<TDb, List<TDbProp>> DbValue;
            public readonly Action<TDbProp, TDb> SetParent;
            public readonly DiffDsl<TNewProp, TDbProp, TChildKey, TUserId> DiffDsl;

            public ManyToManyNode(
                Func<TNewProp, TChildKey> newPropKey,
                Func<TNew, IEnumerable<TNewProp>> newProp,
                Func<TDb, List<TDbProp>> dbValue,
                Action<TDbProp, TDb> setParent,
                DiffDsl<TNewProp, TDbProp, TChildKey, TUserId> diffDsl)
            {
                this.NewPropKey = newPropKey;
                this.NewProp = newProp;
                this.DbValue = dbValue;
                this.SetParent = setParent;
                this.DiffDsl = diffDsl;
            }

            public IUpdateManyToMany<TNew, TDb, TKey, TUserId> Build(IdPointer id)
            {
                var updateManyToMany = new UpdateManyToMany<
                    TNew,
                    TDb,
                    TNewProp,
                    TDbProp,
                    TKey,
                    TChildKey,
                    TUserId>(
                    id.Id++,
                    DiffDsl.immutable,
                    this.NewPropKey,
                    this.NewProp,
                    this.DbValue,
                    this.DiffDsl.updateValues,
                    this.SetParent,
                    this.DiffDsl.diffRepository);
                DiffDsl.Children(
                    id,
                    updateManyToMany);
                return updateManyToMany;
            }
        }
    }

    public class IdPointer
    {
        public int Id = 0;
    }
}