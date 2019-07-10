using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mrh.Core;

namespace Mrh.Database.Diff
{
    public class UpdateManyToOne<
        TNew,
        TDbValue,
        TNewProp,
        TDbProp,
        TKey,
        TChildKey,
        TUserId> : 
        BaseNode<TNewProp, TDbProp, TChildKey, TUserId>,
        IUpdateManyToOne<TNew, TDbValue, TKey, TUserId> 
        where TDbValue : AbstractDatabaseRecord<TKey>
        where TDbProp : AbstractDatabaseRecord<TChildKey>, new()
        where TNewProp : class

    {
        private readonly bool immutable;
        
        private readonly Func<TNew, TNewProp> newProp;

        private readonly Func<TDbValue, TDbProp> dbValue;

        private readonly Action<TDbValue, TDbProp> setDbValue;

        private readonly IEnumerable<IUpdateValue<TNewProp, TDbProp, TChildKey>> childValues;

        private readonly IDiffRepository<TUserId, TChildKey, TDbProp> diffRepository;

        public UpdateManyToOne(
            int nodeId,
            bool immutable,
            Func<TNew, TNewProp> newProp,
            Expression<Func<TDbValue, TDbProp>> dbValue,
            IEnumerable<IUpdateValue<TNewProp, TDbProp, TChildKey>> childValues,
            IDiffRepository<TUserId, TChildKey, TDbProp> diffRepository) : base(nodeId)
        {
            this.immutable = immutable;
            this.newProp = newProp;
            this.dbValue = dbValue.Compile();
            this.setDbValue = ExpressionUtils.CreateSetter(dbValue);
            this.childValues = childValues;
            this.diffRepository = diffRepository;
        }

        public bool Immutable => this.immutable;

        public bool Update(
            TNew newValue,
            TDbValue value,
            Dictionary<int, IUpdateRecords<TUserId>> updateValues)
        {
            var newModel = this.newProp(newValue);
            var dbValue = this.dbValue(value);
            if (newModel == null)
            {
                this.setDbValue(value, null);
                return true;
            }

            if (dbValue == null)
            {
                dbValue = new TDbProp();
            }

            var changed = false;
            foreach (var updateValue in childValues)
            {
                changed = updateValue.Update(newModel, dbValue) || changed;
            }

            if (changed)
            {
                UpdateRecordImpl<TUserId, TDbProp, TChildKey> updateNode;
                if (updateValues.TryGetValue(this.NodeId, out var node))
                {
                    updateNode = (UpdateRecordImpl<TUserId, TDbProp, TChildKey>) node;
                }
                else
                {
                    updateNode = new UpdateRecordImpl<TUserId, TDbProp, TChildKey>(this.NodeId, this.diffRepository);
                }

                if (this.immutable)
                {
                    updateNode.Add(dbValue);
                    return true;
                }
                else
                {
                    updateNode.Update(dbValue);
                    return false;
                }
            }
            return changed;
        }
    }
}