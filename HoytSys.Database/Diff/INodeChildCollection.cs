using System.Collections.Generic;

namespace A19.Database.Diff
{
    public interface INodeChildCollection<TNew, TDb, TKey, TUserId>
        where TDb: AbstractDatabaseRecord<TKey, TNew>
    {
        
        IEnumerable<IUpdateManyToOne<TNew, TDb, TKey, TUserId>> ManyToOnes { get; }
        
        IEnumerable<IUpdateManyToMany<TNew, TDb, TKey, TUserId>> ManyToMany { get; }

        void AddManyToOne(IUpdateManyToOne<TNew, TDb, TKey, TUserId> manyToOne);
        
        void AddManyToMany(IUpdateManyToMany<TNew, TDb, TKey, TUserId> manyToMany);
        
    }
}