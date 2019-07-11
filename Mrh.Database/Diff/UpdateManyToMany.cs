using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mrh.Core;

namespace Mrh.Database.Diff
{
    public class UpdateManyToMany<
        TNew,
        TDbValue,
        TNewProp,
        TDbProp,
        TKey,
        TChildKey,
        TUserId> :
        BaseNode<TNewProp, TDbProp, TChildKey, TUserId>,
        IUpdateManyToMany<TNew, TDbValue, TKey, TUserId>
        where TDbValue : AbstractDatabaseRecord<TKey>
        where TDbProp : AbstractDatabaseRecord<TChildKey>, new()
        where TNewProp : class
    {
        private readonly bool immutable;

        private readonly Func<TNew, IEnumerable<TNewProp>> newProp;

        private readonly Func<TDbValue, List<TDbProp>> dbValue;

        private readonly IEnumerable<IUpdateValue<TNewProp, TDbProp, TChildKey>> childValues;

        private readonly IDiffRepository<TUserId, TChildKey, TDbProp> diffRepository;

        private readonly Func<TNewProp, TChildKey> newPropKey;

        private readonly Action<TDbProp, TDbValue> setParent;

        public UpdateManyToMany(
            int nodeId,
            bool immutable,
            Func<TNewProp, TChildKey> newPropKey,
            Func<TNew, IEnumerable<TNewProp>> newProp,
            Func<TDbValue, List<TDbProp>> dbValue,
            IEnumerable<IUpdateValue<TNewProp, TDbProp, TChildKey>> childValues,
            Action<TDbProp, TDbValue> setParent,
            IDiffRepository<TUserId, TChildKey, TDbProp> diffRepository) : base(nodeId)
        {
            this.immutable = immutable;
            this.newProp = newProp;
            this.dbValue = dbValue;
            this.childValues = childValues;
            this.diffRepository = diffRepository;
            this.newPropKey = newPropKey;
            this.setParent = setParent;
        }

        public bool Immutable => immutable;

        public bool Update(
            TNew newValue,
            TDbValue value,
            Dictionary<int, IUpdateRecords<TUserId>> updateValues)
        {
            var changed = false;
            UpdateRecordImpl<TUserId, TDbProp, TChildKey> updateNode;
            if (updateValues.TryGetValue(this.NodeId, out var temp))
            {
                updateNode = (UpdateRecordImpl<TUserId, TDbProp, TChildKey>) temp;
            }
            else
            {
                updateNode = new UpdateRecordImpl<TUserId, TDbProp, TChildKey>(
                    this.NodeId,
                    this.diffRepository);
                updateValues[this.NodeId] = updateNode;
            }

            var newValues = this.newProp(newValue);
            var dbValues = this.dbValue(value).ToKeyValue(
                (v) => v.Id);
            var hashSetSeen = new HashSet<TChildKey>();
            foreach (var nV in newValues)
            {
                var newKey = this.newPropKey(nV);
                if (dbValues.TryGetValue(newKey, out var dbValue))
                {
                    if (!hashSetSeen.Contains(newKey))
                    {
                        hashSetSeen.Add(newKey);
                    }

                    if (this.UpdateValues(nV, dbValue)
                        || value.IsNew())
                    {
                        if (this.immutable)
                        {
                            dbValue.NewRecord();
                            updateNode.Add(dbValue);
                        }
                        else
                        {
                            dbValue.UpdateRecord();
                            updateNode.Update(dbValue);
                        }
                    }
                }
                else
                {
                    dbValue = new TDbProp();
                    dbValue.NewRecord();
                    this.UpdateValues(nV, dbValue);
                    this.setParent(dbValue, value);
                    updateNode.Add(dbValue);
                }

                this.RunManyToOneUpdate(
                    nV,
                    dbValue,
                    updateValues);
                this.RunManyToManyUpdate(
                    nV,
                    dbValue,
                    updateValues);
            }

            var dbSet = new HashSet<TChildKey>(dbValues.Keys);
            dbSet.ExceptWith(hashSetSeen);
            foreach (var key in dbSet)
            {
                updateNode.Delete(dbValues[key]);
            }

            return false;
        }

        private bool UpdateValues(
            TNewProp newProp,
            TDbProp dbProp)
        {
            var changed = false;
            foreach (var child in this.childValues)
            {
                changed = child.Update(newProp, dbProp) || changed;
            }

            return changed;
        }
    }
}