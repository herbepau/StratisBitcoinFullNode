using System;
using System.Linq;
using Moq;
using Xunit;

namespace Stratis.Bitcoin.Statistics.Tests
{
    public class StatisticsCategoryTests
    {
        [Fact]
        public void Test_CategoryName_is_assigned()
        {
            const string name = "name";
            var category = new StatisticsCategory(name);

            Assert.Equal(name, category.CategoryName);
        }

        [Fact]
        public void Test_statistic_is_added()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory(isAny);

            category.AddOrUpdate(new Statistic("name", isAny));

            Assert.Single(category.Statistics);
        }

        [Fact]
        public void Test_more_than_one_statistic_is_added()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory(isAny);

            category.AddOrUpdate(new Statistic("x", isAny));
            category.AddOrUpdate(new Statistic("y", isAny));

            Assert.Equal(2, category.Statistics.Count());
        }

        [Fact]
        public void Test_statistic_with_same_name_is_not_added()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory(isAny);

            var statistic = new Statistic("x", isAny);

            category.AddOrUpdate(statistic);
            category.AddOrUpdate(statistic);

            Assert.Single(category.Statistics);
        }

        [Fact]
        public void Test_statistic_is_updated()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory(isAny);
            
            category.AddOrUpdate(new Statistic("x", isAny));
            var newStatistic = new Statistic("x", isAny);
            category.AddOrUpdate(newStatistic);

            Assert.True(ReferenceEquals(newStatistic, category.Statistics.First()));
        }
    }
}
