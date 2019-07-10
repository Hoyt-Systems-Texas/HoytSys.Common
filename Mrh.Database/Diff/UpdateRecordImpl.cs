using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mrh.Database.Diff
{
    public class UpdateRecordImpl<TUserId, TDb, TKey> : IUpdateRecords<TUserId>
        where TDb: AbstractDatabaseRecord<TKey>

    {
        private readonly int nodeId;
        private readonly IDiffRepository<TUserId, TKey, TDb> diffRepository;
        private readonly List<TDb> add = new List<TDb>(2);
        private readonly List<TDb> update = new List<TDb>(2);
        private readonly List<TDb> delete = new List<TDb>(2);

        public UpdateRecordImpl(
            int nodeId,
            IDiffRepository<TUserId, TKey, TDb> diffRepository)
        {
            this.nodeId = nodeId;
            this.diffRepository = diffRepository;
        }

        public int NodeId => nodeId;
        
        public async Task Run(TUserId userId)
        {
            if (this.add.Count > 0)
            {
                await this.diffRepository.Add(this.add, userId);
            }

            if (this.update.Count > 0)
            {
                await this.diffRepository.Update(this.update, userId);
            }

            if (this.delete.Count > 0)
            {
                await this.diffRepository.Delete(this.delete, userId);
            }
        }

        public void Add(TDb value)
        {
            this.add.Add(value);
        }

        public void Delete(TDb value)
        {
            this.delete.Add(value);
        }

        public void Update(TDb value)
        {
            this.update.Add(value);
        }
    }
}