using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DistinctByUt
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

    public class DistinctByTest
    {
        private IEnumerable<Data> repeatOnlyOne = Enumerable.Repeat(55, 100).Select(i => new Data() { D = i });
        private IEnumerable<Data> repeatTwo = Enumerable.Repeat(55, 10).Union(Enumerable.Repeat(88, 90)).Select(i => new Data() { D = i });
        private IEnumerable<Data> range = Enumerable.Range(0, 10).Union(Enumerable.Range(0, 90)).Select(i => new Data() { D = i });

        [Fact]
        public void RepeatOnlyOneShouldBeOne()
        {
            var datas = repeatOnlyOne.DistinctBy(i => i.D).ToList();
            Assert.Single(datas);
            Assert.Equal(55, datas.First().D);
        }

        [Fact]
        public void RepeatTwoShouldBeTwo()
        {
            var datas = repeatTwo.DistinctBy(i => i.D).ToList();
            Assert.Equal(2, datas.Count);
            Assert.Equal(55, datas.First().D);
            Assert.Equal(88, datas[1].D);
        }

        [Fact]
        public void RangeShouldBeTen()
        {
            var datas = range.DistinctBy(i => i.D).ToList();
            Assert.Equal(90, datas.Count);
            for (int i = 0; i < datas.Count; i++)
            {
                Assert.Equal(i, datas[i].D);
            }
        }
    }
}