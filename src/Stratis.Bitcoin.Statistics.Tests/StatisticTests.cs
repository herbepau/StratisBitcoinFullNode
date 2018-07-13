using System;
using Moq;
using Xunit;

namespace Stratis.Bitcoin.Statistics.Tests
{
    public class StatisticTests
    {
        [Fact]
        public void Test_exception_thrown_when_name_is_null()
        {
            Assert.Throws<ArgumentException>(() => new Statistic(null, It.IsAny<string>()));
        }

        [Fact]
        public void Test_exception_thrown_when_name_is_empty()
        {
            Assert.Throws<ArgumentException>(() => new Statistic(string.Empty, It.IsAny<string>()));
        }

        [Fact]
        public void Test_Name_is_assigned()
        {
            const string name = "name";
            var statistic = new Statistic(name, string.Empty);

            Assert.Equal(name, statistic.Name);
        }

        [Fact]
        public void Test_Value_is_assigned()
        {
            const string value = "value";
            var statistic = new Statistic("name", value);

            Assert.Equal(value, statistic.Value);
        }

        [Fact]
        public void Test_Statistics_is_assigned()
        {            
            var statistic = new Statistic("name", string.Empty, new [] { new Statistic("name", string.Empty) });

            Assert.Single(statistic.Statistics);
        }
    }
}
