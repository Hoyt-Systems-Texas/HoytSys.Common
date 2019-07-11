using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mrh.Database.Diff
{
    /// <summary>
    ///     The diff DSL library for generating a comparision. 
    /// </summary>
    /// <typeparam name="TNew"></typeparam>
    /// <typeparam name="TDb"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TUserId"></typeparam>
    public class DiffDsl<TNew, TDb, TKey, TUserId> where TDb: AbstractDatabaseRecord<TKey>
    {

        private readonly IDiffRepository<TUserId, TKey, TDb> diffRepository;
        private readonly List<IUpdateValue<TNew, TDb, TKey>> updateValues = new List<IUpdateValue<TNew, TDb, TKey>>(10);

        public DiffDsl(
            IDiffRepository<TUserId, TKey, TDb> diffRepository)
        {
            this.diffRepository = diffRepository;
        }

        public DiffDsl<TNew, TDb, TKey, TUserId> Add<TProperty>(
            Func<TNew, TProperty> newValues,
            Expression<Func<TDb, TProperty>> dbValue,
            Func<TProperty, TProperty, bool> func)
        {
            this.updateValues.Add(
                new FieldUpdateValue<TNew,TDb,TProperty,TKey,TUserId>(
                    newValues,
                    dbValue,
                    func));
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
            Expression<Func<TDb,long>> dbValue)
        {
            return this.Add(
                newValues,
                dbValue,
                (i1, i2) => i1 == i2);
        }
        
        public DiffDsl<TNew, TDb, TKey, TUserId> Add(
            Func<TNew, long?> newValues,
            Expression<Func<TDb,long?>> dbValue)
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
    }
}