using System.Collections.Generic;
using System.Linq;
using Moq;
using Stratis.Bitcoin.Statistics.Interfaces;
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
        public void Test_statistics_are_assigned_during_construction()
        {
            string isAny = It.IsAny<string>();
            var stats = new List<IStatistic> {new Statistic("x", isAny)};

            var category = new StatisticsCategory(isAny, stats);

            Assert.True(category.Statistics.SequenceEqual(stats));
        }

        [Fact]
        public void Test_Apply_adds_statistic()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory(isAny);
            category.Apply(new Statistic("name", isAny));

            Assert.Single(category.Statistics);
        }

        [Fact]
        public void Test_Apply_adds_more_than_one_statistic()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory(isAny);

            category.Apply(new Statistic("x", isAny));
            category.Apply(new Statistic("y", isAny));

            Assert.Equal(2, category.Statistics.Count());
        }

        [Fact]
        public void Test_Apply_adds_same_named_stat_only_once()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory(isAny);

            var statistic = new Statistic("x", isAny);

            category.Apply(statistic);
            category.Apply(statistic);

            Assert.Single(category.Statistics);
        }        

        [Fact]
        public void Test_Apply_updates_where_category_name_is_the_same()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory(isAny);
            
            category.Apply(new Statistic("x", isAny));
            var newStatistic = new Statistic("x", isAny);
            category.Apply(newStatistic);

            Assert.True(ReferenceEquals(newStatistic, category.Statistics.First()));
        }

        [Fact]
        public void Test_Statistics_are_added_when_passed_at_construction()
        {
            string isAny = It.IsAny<string>();
            var statistics = new List<IStatistic>
            {
                new Statistic("x", isAny),
                new Statistic("y", isAny)
            };
            var category = new StatisticsCategory(isAny, statistics);

            Assert.True(category.Statistics.SequenceEqual(statistics));
        }
    }
}
