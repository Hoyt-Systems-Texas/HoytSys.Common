using System.Collections.Generic;
using Mrh.DataStructures.Graph;

namespace Mrh.Database.Diff
{
    public interface INodeChildCollection<TNew, TDb, TKey, TUserId>
        where TDb: AbstractDatabaseRecord<TKey>
    {
        
        IEnumerable<IUpdateManyToOne<TNew, TDb, TKey, TUserId>> ManyToOnes { get; }
        
        IEnumerable<IUpdateManyToMany<TNew, TDb, TKey, TUserId>> ManyToMany { get; }

        void AddManyToOne(IUpdateManyToOne<TNew, TDb, TKey, TUserId> manyToOne);
        
        void AddManyToMany(IUpdateManyToMany<TNew, TDb, TKey, TUserId> manyToMany);
        
    }
}