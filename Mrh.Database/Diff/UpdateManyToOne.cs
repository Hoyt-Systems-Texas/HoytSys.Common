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
        TUserId> : IUpdateValue<
        TNew,
        TDbValue,
        TKey> where TDbValue : AbstractDatabaseRecord<TKey>
        where TDbProp : AbstractDatabaseRecord<TChildKey>, new()
        where TNewProp : class

    {
        private readonly Func<TNew, TNewProp> newProp;

        private readonly Func<TDbValue, TDbProp> dbValue;

        private readonly Action<TDbValue, TDbProp> setDbValue;

        private readonly IEnumerable<IUpdateValue<TNewProp, TDbProp, TChildKey>> childValues;

        private readonly IDiffRepository<TUserId, TKey, TDbValue> diffRepository;

        public UpdateManyToOne(
            Func<TNew, TNewProp> newProp,
            Expression<Func<TDbValue, TDbProp>> dbValue,
            IEnumerable<IUpdateValue<TNewProp, TDbProp, TChildKey>> childValues)
        {
            this.newProp = newProp;
            this.dbValue = dbValue.Compile();
            this.setDbValue = ExpressionUtils.CreateSetter(dbValue);
            this.childValues = childValues;
        }

        public UpdateRecordType Update(TNew newValue, TDbValue value)
        {
            var newPropValue = this.newProp(newValue);
            var oldPropValue = this.dbValue(value);
            if (newValue == null
                && oldPropValue == null)
            {
                return UpdateRecordType.Same;
            }
            else if (newValue == null)
            {
                if (oldPropValue.DeleteRecord())
                {
                    return UpdateRecordType.ManyToOne;
                }
                else
                {
                    return UpdateRecordType.Same;
                }
            }
            else if (oldPropValue == null)
            {
                oldPropValue = new TDbProp();
                oldPropValue.NewRecord();
            }

            var valueChanged = false;
            foreach (var child in this.childValues)
            {
                // Order is important.
            }
            return UpdateRecordType.ManyToOne;
        }
    }
}