using System;
using System.Linq.Expressions;
using HoytSys.Core;

namespace A19.Database.Diff
{
    
    /// <summary>
    ///     Represents updating a single field value.
    /// </summary>
    /// <typeparam name="TEntity">The entity to update.</typeparam>
    /// <typeparam name="TDbValue">The db value to update.</typeparam>
    /// <typeparam name="TProperty">The property type of the value to update.</typeparam>
    /// <typeparam name="TKey">The key to update.</typeparam>
    public class FieldUpdateValue<TEntity, TDbValue, TProperty, TKey, TUserId> : IUpdateValue<
    TEntity, TDbValue, TKey> where TDbValue:AbstractDatabaseRecord<TKey, TEntity>
    {

        private readonly Func<TEntity, TProperty> newValue;

        private readonly Func<TDbValue, TProperty> dbValue;

        private readonly Func<TProperty, TProperty, bool> compareValue;

        private readonly Action<TDbValue, TProperty> setValue;

        public FieldUpdateValue(
            Func<TEntity, TProperty> newValue,
            Expression<Func<TDbValue, TProperty>> dbValue,
            Func<TProperty, TProperty, bool> compareValue)
        {
            this.newValue = newValue;
             this.dbValue = dbValue.Compile();
             this.compareValue = compareValue;
             this.setValue = ExpressionUtils.CreateSetter(dbValue);
         }
 
         public bool Update(TEntity newValue, TDbValue value)
         {
             var newProp = this.newValue(newValue);
             var oldProp = this.dbValue(value);
             var compareResults = this.compareValue(newProp, oldProp);
             if (!compareResults)
             {
                 this.setValue(value, newProp);
                 return true;
             }
             return false;
         }
     }
 }