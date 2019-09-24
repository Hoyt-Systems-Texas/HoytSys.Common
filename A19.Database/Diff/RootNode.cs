using System.Collections.Generic;

namespace A19.Database.Diff
{
    public class RootNode<TNew, TDb, TKey, TUserId> : BaseNode<TNew, TDb, TKey, TUserId>,
        IUpdateRecordValue<TNew, TDb, TKey, TUserId> where TDb: AbstractDatabaseRecord<TKey, TNew>, new()
    {

        private readonly IDiffRepository<TUserId, TKey, TDb, TNew> diffRepository;
        private readonly List<IUpdateValue<TNew, TDb, TKey>> updateValues;
        private readonly bool immutable;
        
        public RootNode(
            int nodeId,
            bool immutable,
            IDiffRepository<TUserId, TKey, TDb, TNew> diffRepository,
            List<IUpdateValue<TNew, TDb, TKey>> updateValues) : base(nodeId)
        {
            this.diffRepository = diffRepository;
            this.immutable = immutable;
            this.updateValues = updateValues;
        }

        public bool Immutable { get; }
        
        public bool Update(
            TNew newValue,
            TDb value,
            Dictionary<int, IUpdateRecords<TUserId>> updateValues)
        {
            UpdateRecordImpl<TUserId, TDb, TKey, TNew> updateRecord;
            if (updateValues.TryGetValue(this.NodeId, out var temp))
            {
                updateRecord = (UpdateRecordImpl<TUserId, TDb, TKey, TNew>) temp;
            }
            else
            {
                updateRecord = new UpdateRecordImpl<TUserId, TDb, TKey, TNew>(
                    this.NodeId,
                    this.diffRepository);
                updateValues[this.NodeId] = updateRecord;
            }

            var changed = false;
            var createdNew = false;
            if (value == null)
            {
                value = new TDb();
                value.NewValue = newValue;
                createdNew = true;
            }
            foreach (var updateValue in this.updateValues)
            {
                changed = updateValue.Update(newValue, value) || changed;
            }

            changed = (this.RunManyToOneUpdate(newValue, value, updateValues) && this.immutable) || changed;
            if (changed)
            {
                if (this.immutable || createdNew)
                {
                    updateRecord.Add(value);
                }
                else
                {
                    updateRecord.Update(value);
                }
            }

            this.RunManyToManyUpdate(newValue, value, updateValues);
            return changed;
        }
    }
}