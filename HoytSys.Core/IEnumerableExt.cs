using System;
using System.Collections.Generic;
using System.Linq;

namespace HoytSys.Core
{
    public static class EnumerableExt
    {
        public static Dictionary<TKey, TValue> ToKeyValue<TKey, TValue>(
            this IEnumerable<TValue> values,
            Func<TValue, TKey> keyFun) where TKey : notnull
        {
            return values.Aggregate(
                new Dictionary<TKey, TValue>(10),
                (dic, value) =>
                {
                    dic[keyFun(value)] = value;
                    return dic;
                });
        }
    }
}