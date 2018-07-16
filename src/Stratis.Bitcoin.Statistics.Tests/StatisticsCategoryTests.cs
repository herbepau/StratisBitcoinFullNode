using System;
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
        public void Test_exception_thrown_when_categoryName_is_null()
        {
            Assert.Throws<ArgumentException>(() => new StatisticsCategory(null));
        }

        [Fact]
        public void Test_exception_thrown_when_categoryName_is_empty()
        {
            Assert.Throws<ArgumentException>(() => new StatisticsCategory(string.Empty));
        }

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
            var stats = new List<IStatistic> {new Statistic("x", It.IsAny<string>()) };
            var category = new StatisticsCategory("x", stats);

            Assert.True(category.Statistics.SequenceEqual(stats));
        }

        [Fact]
        public void Test_Apply_adds_statistic()
        {            
            var category = new StatisticsCategory("x");
            category.Apply(new Statistic("name", It.IsAny<string>()));

            Assert.Single(category.Statistics);
        }

        [Fact]
        public void Test_Apply_adds_more_than_one_statistic()
        {
            string isAny = It.IsAny<string>();
            var category = new StatisticsCategory("x");

            category.Apply(new Statistic("x", isAny));
            category.Apply(new Statistic("y", isAny));

            Assert.Equal(2, category.Statistics.Count());
        }

        [Fact]
        public void Test_Apply_adds_same_named_stat_only_once()
        {            
            var category = new StatisticsCategory("x");

            var statistic = new Statistic("x", It.IsAny<string>());

            category.Apply(statistic);
            category.Apply(statistic);

            Assert.Single(category.Statistics);
        }        

        [Fact]
        public void Test_Apply_updates_where_category_name_is_the_same()
        {            
            var category = new StatisticsCategory("x");
            const string newValue = "123";
            
            category.Apply(new Statistic("x", It.IsAny<string>()));            
            category.Apply(new Statistic("x", newValue));

            Assert.True(category.Statistics.Single().Value == newValue);
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
            var category = new StatisticsCategory("x", statistics);

            Assert.True(category.Statistics.SequenceEqual(statistics));
        }

        [Fact]
        public void Test_ChangeStream_pumps_when_statistic_is_added()
        {
            var category = new StatisticsCategory("x");
            var pumped = false;
            category.ChangedStream.Subscribe(_ => pumped = true);
            category.Apply(new Statistic("x", string.Empty));    
            Assert.True(pumped);
        }

        [Fact]
        public void Test_ChangeStream_pumps_when_statistic_value_is_changed()
        {
            var category = new StatisticsCategory("x");
            var pumped = false;
            category.ChangedStream.Subscribe(_ => pumped = true);
            category.Apply(new Statistic("x", string.Empty));
            category.Apply(new Statistic("x", "1"));
            Assert.True(pumped);
        }

        [Fact]
        public void Test_ChangeStream_does_not_pump_when_value_is_the_same()
        {
            var category = new StatisticsCategory("x");
            var pumped = false;
            category.Apply(new Statistic("x", string.Empty));
            category.ChangedStream.Subscribe(_ => pumped = true);            
            category.Apply(new Statistic("x", string.Empty));
            Assert.False(pumped);
        }
    }
}
