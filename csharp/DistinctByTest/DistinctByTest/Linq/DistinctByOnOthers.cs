using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DistinctByTest.Linq
{
    public static partial class EnumerableExtensions
    {
        public static IEnumerable<T> ReplaceIfEmpty<T>(this IEnumerable<T> source, IEnumerable<T> replaceSource)
        {
            var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                yield return enumerator.Current;
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
            else
            {
                foreach (var item in replaceSource)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> LazyReplaceIfEmpty<T>(this IEnumerable<T> source, Func<IEnumerable<T>> replaceSource)
        {
            var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                yield return enumerator.Current;
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
            else
            {
                foreach (var item in replaceSource())
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<TSource> FristIfEmptyUseSecond<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> second, Func<TSource, bool> filter)
        {
            return source.Where(filter).ReplaceIfEmpty(second.Where(filter));
        }

        public static IEnumerable<TSource> DistinctByOnGroupBy<TSource, TDistinctBy>(this IEnumerable<TSource> source, Func<TSource, TDistinctBy> getDistinctBy)
        {
            return source.GroupBy(getDistinctBy).Select(g => g.First());
        }

        public static IEnumerable<TSource> DistinctByOnlyHashSet<TSource, TDistinctBy>(this IEnumerable<TSource> source, Func<TSource, TDistinctBy> getDistinctBy)
        {
            HashSet<TDistinctBy> seenKeys = new HashSet<TDistinctBy>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(getDistinctBy(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> DistinctByOnDistinctByEqualityComparer<TSource, TDistinctBy>(this IEnumerable<TSource> source, Func<TSource, TDistinctBy> getDistinctBy)
        {
            return source.Distinct(new DistinctByEqualityComparer<TSource, TDistinctBy>(getDistinctBy));
        }

        private class DistinctByEqualityComparer<T, R> : IEqualityComparer<T>
        {
            private readonly Func<T, R> getDistinctBy;

            public DistinctByEqualityComparer(Func<T, R> getDistinctBy)
            {
                this.getDistinctBy = getDistinctBy;
            }

            public bool Equals([AllowNull] T x, [AllowNull] T y)
            {
                if (Object.ReferenceEquals(x, y)) return true;
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;
                var xv = getDistinctBy(x);
                var yv = getDistinctBy(y);
                return Object.ReferenceEquals(xv,yv);
            }

            public int GetHashCode([DisallowNull] T obj)
            {
                if (obj == null) return 0;
                var v = getDistinctBy(obj);
                return v == null ? 0 : v.GetHashCode();
            }
        }
    }
}