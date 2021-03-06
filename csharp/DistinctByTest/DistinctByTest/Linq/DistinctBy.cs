﻿using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class EnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TDistinctBy>(this IEnumerable<TSource> source, Func<TSource, TDistinctBy> getDistinctBy) => DistinctBy(source, getDistinctBy, EqualityComparer<TDistinctBy>.Default);

        public static IEnumerable<TSource> DistinctBy<TSource, TDistinctBy>(this IEnumerable<TSource> source, Func<TSource, TDistinctBy> getDistinctBy, IEqualityComparer<TDistinctBy> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new DistinctByIterator<TSource, TDistinctBy>(source, getDistinctBy, comparer);
        }

        private sealed class DistinctByIterator<TSource, TDistinctBy> : IEnumerable<TSource>, IEnumerator<TSource>
        {
            private int state;
            private readonly IEnumerable<TSource> source;
            private readonly Func<TSource, TDistinctBy> getDistinctBy;
            private readonly IEqualityComparer<TDistinctBy> comparer;
            private TDistinctBy first;
            private TSource current;
            private HashSet<TDistinctBy> set;
            private IEnumerator<TSource> iterator;

            public DistinctByIterator(IEnumerable<TSource> source, Func<TSource, TDistinctBy> getDistinctBy, IEqualityComparer<TDistinctBy> comparer)
            {
                this.source = source;
                this.getDistinctBy = getDistinctBy;
                this.comparer = comparer;
            }

            public TSource Current => current;

            object IEnumerator.Current => current;

            public void Dispose()
            {
                state = 3;
                first = default;
                iterator = null;
                current = default;
                set = null;
            }

            public bool MoveNext()
            {
                switch (state)
                {
                    case 0:
                        iterator = source.GetEnumerator();
                        if (!iterator.MoveNext())
                        {
                            Dispose();
                            return false;
                        }
                        current = iterator.Current;
                        state = 1;
                        return true;

                    case 1:
                        first = getDistinctBy(current);
                        var firstHashCode = first.GetHashCode();
                        if (!iterator.MoveNext())
                        {
                            Dispose();
                            return false;
                        }
                        current = iterator.Current;
                        var next = getDistinctBy(current);
                        while (firstHashCode == next.GetHashCode() && comparer.Equals(first, next))
                        {
                            if (!iterator.MoveNext())
                            {
                                Dispose();
                                return false;
                            }
                            current = iterator.Current; 
                            next = getDistinctBy(current);
                        }
                        state = 2;
                        return true;

                    case 2:
                        var second = getDistinctBy(current);
                        if (!iterator.MoveNext())
                        {
                            Dispose();
                            return false;
                        }
                        set = new HashSet<TDistinctBy>(comparer)
                        {
                            first,
                            second
                        };
                        current = iterator.Current;
                        var currentDistinctBy = getDistinctBy(current);
                        while (!set.Add(currentDistinctBy))
                        {
                            if (!iterator.MoveNext())
                            {
                                Dispose();
                                return false;
                            }
                            current = iterator.Current;
                            currentDistinctBy = getDistinctBy(current);
                        }
                        state = 3;
                        return true;
                    case 3:
                        if (!iterator.MoveNext())
                        {
                            Dispose();
                            return false;
                        }
                        current = iterator.Current;
                        currentDistinctBy = getDistinctBy(current);
                        while (!set.Add(currentDistinctBy))
                        {
                            if (!iterator.MoveNext())
                            {
                                Dispose();
                                return false;
                            }
                            current = iterator.Current;
                            currentDistinctBy = getDistinctBy(current);
                        }
                        return true;

                    default:
                        return false;
                }
            }

            public void Reset()
            {
                Dispose();
                state = 0;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                return new DistinctByIterator<TSource, TDistinctBy>(source, getDistinctBy, comparer);
            }
        }
    }
}