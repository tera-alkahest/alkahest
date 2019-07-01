using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Core.Collections
{
    public static class CollectionExtensions
    {
        public static IEnumerable<(TKey, TValue)> Tuples<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Select(kvp => (kvp.Key, kvp.Value));
        }

        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Select((i, x) => (x, i));
        }
    }
}
