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
    public class FieldUpdateValue<TEntity, TDbValue, TProperty, TKey, TUserId> : IUpdateValue<
    TEntity, TDbValue, TKey> where TDbValue:AbstractDatabaseRecord<TKey>
    {

        private readonly Func<TEntity, TProperty> newValue;

        private readonly Func<TDbValue, TProperty> dbValue;

        private readonly Func<TProperty, TProperty, UpdateRecordType> compareValue;

        private readonly Action<TDbValue, TProperty> setValue;

        private readonly IDiffRepository<TUserId, TKey, TDbValue> diffRepository;
        
        private FieldUpdateValue(
            Func<TEntity, TProperty> newValue,
            Expression<Func<TDbValue, TProperty>> dbValue,
            Func<TProperty, TProperty, UpdateRecordType> compareValue,
            IDiffRepository<TUserId, TKey, TDbValue> diffRepository)
        {
            this.newValue = newValue;
             this.dbValue = dbValue.Compile();
             this.compareValue = compareValue;
             this.setValue = ExpressionUtils.CreateSetter(dbValue);
             this.diffRepository = diffRepository;
         }
 
         public UpdateRecordType Update(TEntity newValue, TDbValue value)
         {
             var newProp = this.newValue(newValue);
             var oldProp = this.dbValue(value);
             var compareResults = this.compareValue(newProp, oldProp);
             switch (compareResults)
             {
                 case UpdateRecordType.Same:
                     break;
                 
                 case UpdateRecordType.Field:
                     break;
                 
                 case UpdateRecordType.ManyToMany:
                     break;
                 
                 case UpdateRecordType.ManyToOne:
                     break;
                 
                 default:
                     throw new Exception($"Unknown result type.");
             }
 
             return compareResults;
         }
     }
 }