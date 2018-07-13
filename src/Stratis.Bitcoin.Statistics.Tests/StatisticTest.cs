using System;

namespace Stratis.Bitcoin.Statistics.Tests
{
    public class StatisticTest
    {
        [Fact]
        public void Test_Name_is_assigned()
        {
            const string Name = "name";
            var statistic = new Statistic(Name, string.Empty);
            Assert.Equal(Name, statistic.Name);
        }

        [Fact]
        public void Test_Value_is_assigned()
        {
            const string Value = "value";
            var statistic = new Statistic(string.Empty, Value);
            Assert.Equal(Value, statistic.Value);
        }
    }
}
