using System;
using System.Collections.Generic;

namespace Alkahest.Core.Collections
{
    public static class CollectionExtensions
    {
        public static IEnumerable<(TKey, TValue)> Tuples<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var kvp in source)
                yield return (kvp.Key, kvp.Value);
        }

        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var i = 0;

            foreach (var item in source)
            {
                yield return (i, item);

                i++;
            }
        }

        public static IEnumerable<(long, T)> WithLongIndex<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            long i = 0;

            foreach (var item in source)
            {
                yield return (i, item);

                i++;
            }
        }
    }
}
