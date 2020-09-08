using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Tools.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return (null == collection) || (0 == collection.Count);
        }

        public static SortedDictionary<K, V> ToSortedDictionary<K, V>(this IDictionary<K, V> dictionary)
        {
            return new SortedDictionary<K, V>(dictionary);
        }

        public static SortedDictionary<K, V> ToSortedDictionary<K, V>(this IDictionary<K, V> dictionary, IComparer<K> comparer)
        {
            return new SortedDictionary<K, V>(dictionary, comparer);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
        {
            return first.Where(x => second.Count(y => comparer(x, y)) == 0);
        }

        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
        {
            return first.Where(x => second.Count(y => comparer(x, y)) == 1);
        }

        public static bool IsIn<T>(this T value, params T[] collection) => collection.Contains(value);

        public static bool In<T>(this T value, IEnumerable<T> collection) => collection.Contains(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIn<T>(this T value, T item1) where T : struct
        {
            return value.Equals(item1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIn<T>(this T value, T item1, T item2) where T : struct
        {
            return (value.Equals(item1) || value.Equals(item2));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIn<T>(this T value, T item1, T item2, T item3) where T : struct
        {
            return (value.Equals(item1) || value.Equals(item2) || value.Equals(item3));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIn<T>(this T value, T item1, T item2, T item3, T item4) where T : struct
        {
            return (value.Equals(item1) || value.Equals(item2) || value.Equals(item3) || value.Equals(item4));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIn<T>(this T value, T item1, T item2, T item3, T item4, T item5) where T : struct
        {
            return (value.Equals(item1) || value.Equals(item2) || value.Equals(item3) || value.Equals(item4) || value.Equals(item5));
        }
        public static string ToFlatString<K, V>(this IDictionary<K, V> source, string prefix = @"[", string postfix = @"]", string keyValueSeparator = @":", string sequenceSeparator = @"|")
        {
            return string.Concat(prefix, string.Join(sequenceSeparator, source.Select(x => string.Concat(x.Key, keyValueSeparator, x.Value))), postfix);
        }
    }
}