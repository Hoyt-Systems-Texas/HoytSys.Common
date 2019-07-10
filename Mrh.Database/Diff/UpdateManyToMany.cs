using System;
using System.Collections.Generic;
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

        private readonly Func<TNew, TNewProp> newProp;

        private readonly Func<TDbValue, TDbProp> dbValue;

        private readonly Action<TDbValue, TDbProp> setDbValue;

        private readonly IEnumerable<IUpdateValue<TNewProp, TDbProp, TChildKey>> childValues;

        private readonly IDiffRepository<TUserId, TChildKey, TDbProp> diffRepository;

        public UpdateManyToMany(
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

        public bool Immutable => Immutable;

        public bool Update(TNew newValue, TDbValue value, Dictionary<int, IUpdateRecords<TUserId>> updateValues)
        {
            throw new System.NotImplementedException();
        }
    }
}