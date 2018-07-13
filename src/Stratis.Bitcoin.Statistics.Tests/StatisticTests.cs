using Xunit;

namespace Stratis.Bitcoin.Statistics.Tests
{
    public class StatisticTests
    {
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
            var statistic = new Statistic(string.Empty, value);

            Assert.Equal(value, statistic.Value);
        }

        [Fact]
        public void Test_Statistics_is_assigned()
        {            
            var statistic = new Statistic(string.Empty, string.Empty, new [] { new Statistic(string.Empty, string.Empty) });

            Assert.Single(statistic.Statistics);
        }
    }
}
