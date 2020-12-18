using BenchmarkDotNet.Attributes;
using DistinctByTest.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DistinctByTest
{
    public class Data : IEquatable<Data>
    {
        public int D { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Data);
        }

        public bool Equals(Data other)
        {
            return other != null &&
                   D == other.D;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(D);
        }

        public static bool operator ==(Data left, Data right)
        {
            return EqualityComparer<Data>.Default.Equals(left, right);
        }

        public static bool operator !=(Data left, Data right)
        {
            return !(left == right);
        }
    }

    [MemoryDiagnoser]
    public class BenmarkTest
    {
        private IEnumerable<Data> repeatOnlyOne = Enumerable.Repeat(55, 100).Select(i => new Data() { D = i });
        private IEnumerable<Data> repeatTwo = Enumerable.Repeat(55, 10).Union(Enumerable.Repeat(88, 90)).Select(i => new Data() { D = i });
        private IEnumerable<Data> range = Enumerable.Range(10, 10).Union(Enumerable.Range(10, 90)).Select(i => new Data() { D = i });

        [Benchmark]
        public void RepeatOnlyOne_Distinct()
        {
            repeatOnlyOne.Select(i => i.D).Distinct().ToList();
        }

        [Benchmark]
        public void RepeatOnlyOne_DistinctBy()
        {
            repeatOnlyOne.DistinctBy(i => i.D).ToList();
        }

        [Benchmark]
        public void RepeatOnlyOne_DistinctByOnGroupBy()
        {
            repeatOnlyOne.DistinctByOnGroupBy(i => i.D).ToList();
        }

        [Benchmark]
        public void RepeatOnlyOne_DistinctByOnlyHashSet()
        {
            repeatOnlyOne.DistinctByOnlyHashSet(i => i.D).ToList();
        }

        [Benchmark]
        public void RepeatOnlyOne_DistinctByOnDistinctByEqualityComparer()
        {
            repeatOnlyOne.DistinctByOnDistinctByEqualityComparer(i => i.D).ToList();
        }

        [Benchmark]
        public void RepeatTwo_Distinct()
        {
            repeatTwo.Select(i => i.D).Distinct().ToList();
        }

        [Benchmark]
        public void RepeatTwo_DistinctBy()
        {
            repeatTwo.DistinctBy(i => i.D).ToList();
        }

        [Benchmark]
        public void RepeatTwo_DistinctByOnGroupBy()
        {
            repeatTwo.DistinctByOnGroupBy(i => i.D).ToList();
        }

        [Benchmark]
        public void RepeatTwo_DistinctByOnlyHashSet()
        {
            repeatTwo.DistinctByOnlyHashSet(i => i.D).ToList();
        }

        [Benchmark]
        public void RepeatTwo_DistinctByOnDistinctByEqualityComparer()
        {
            repeatTwo.DistinctByOnDistinctByEqualityComparer(i => i.D).ToList();
        }

        [Benchmark]
        public void Range_Distinct()
        {
            range.Select(i => i.D).Distinct().ToList();
        }

        [Benchmark]
        public void Range_DistinctBy()
        {
            range.DistinctBy(i => i.D).ToList();
        }

        [Benchmark]
        public void Range_DistinctByOnGroupBy()
        {
            range.DistinctByOnGroupBy(i => i.D).ToList();
        }

        [Benchmark]
        public void Range_DistinctByOnlyHashSet()
        {
            range.DistinctByOnlyHashSet(i => i.D).ToList();
        }

        [Benchmark]
        public void Range_DistinctByOnDistinctByEqualityComparer()
        {
            range.DistinctByOnDistinctByEqualityComparer(i => i.D).ToList();
        }
    }
}