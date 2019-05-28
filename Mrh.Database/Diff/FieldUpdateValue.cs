using System;
using System.Linq.Expressions;
using Mrh.Core;

namespace Mrh.Database.Diff
{
    
    /// <summary>
    ///     Represents updating a single field value.
    /// </summary>
    /// <typeparam name="TEntity">The entity to update.</typeparam>
    /// <typeparam name="TDbValue">The db value to update.</typeparam>
    /// <typeparam name="TProperty">The property type of the value to update.</typeparam>
    /// <typeparam name="TKey">The key to update.</typeparam>
    public class FieldUpdateValue<TEntity, TDbValue, TProperty, TKey> : IUpdateValue<
    TEntity, TDbValue, TKey> where TDbValue:AbstractDatabaseRecord<TKey>
    {

        private readonly Func<TEntity, TProperty> newValue;

        private readonly Func<TDbValue, TProperty> dbValue;

        private readonly Func<TProperty, TProperty, bool> compareValue;

        private readonly Action<TDbValue, TProperty> setValue;
        
        private FieldUpdateValue(
            Func<TEntity, TProperty> newValue,
            Expression<Func<TDbValue, TProperty>> dbValue,
            Func<TProperty, TProperty, bool> compareValue)
        {
            this.newValue = newValue;
            this.dbValue = dbValue.Compile();
            this.compareValue = compareValue;
            this.setValue = ExpressionUtils.CreateSetter(dbValue);
        }

        public UpdateRecordType Update(TEntity newValue, TDbValue value)
        {
            var newProp = this.newValue(newValue);
            var oldProp = this.dbValue(value);
            if (this.compareValue(newProp, oldProp))
            {
                return UpdateRecordType.Same;
            }
            else
            {
                this.setValue(value, newProp);
                return UpdateRecordType.Field;
            }
        }
    }
}