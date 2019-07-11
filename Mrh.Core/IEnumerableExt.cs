using System;
using System.Collections.Generic;
using System.Linq;

namespace Mrh.Core
{
    public static class IEnumerableExt
    {
        public static Dictionary<TKey, TValue> ToKeyValue<TKey, TValue>(
            this IEnumerable<TValue> values,
            Func<TValue, TKey> keyFun)
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