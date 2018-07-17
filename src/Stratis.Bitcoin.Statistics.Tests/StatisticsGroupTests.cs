using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Stratis.Bitcoin.Statistics.Interfaces;
using Xunit;

namespace Stratis.Bitcoin.Statistics.Tests
{
    public class StatisticsGroupTests
    {
        [Fact]
        public void Test_exception_thrown_when_groupName_is_null()
        {
            Assert.Throws<ArgumentException>(() => new StatisticGroup(null));
        }

        [Fact]
        public void Test_exception_thrown_when_groupName_is_empty()
        {
            Assert.Throws<ArgumentException>(() => new StatisticGroup(string.Empty));
        }

        [Fact]
        public void Test_GroupName_is_assigned()
        {
            const string name = "name";
            var group = new StatisticGroup(name);

            Assert.Equal(name, group.GroupName);
        }

        [Fact]
        public void Test_statistics_are_assigned_during_construction()
        {            
            var stats = new List<IStatistic> {new Statistic("x", It.IsAny<string>()) };
            var group = new StatisticGroup("x", stats);

            Assert.True(group.Statistics.SequenceEqual(stats));
        }

        [Fact]
        public void Test_Apply_adds_statistic()
        {            
            var group = new StatisticGroup("x");
            group.Apply(new Statistic("name", It.IsAny<string>()));

            Assert.Single(group.Statistics);
        }

        [Fact]
        public void Test_Apply_adds_more_than_one_statistic()
        {
            string isAny = It.IsAny<string>();
            var group = new StatisticGroup("x");

            group.Apply(new Statistic("x", isAny));
            group.Apply(new Statistic("y", isAny));

            Assert.Equal(2, group.Statistics.Count());
        }

        [Fact]
        public void Test_Apply_adds_same_named_stat_only_once()
        {            
            var group = new StatisticGroup("x");

            var statistic = new Statistic("x", It.IsAny<string>());

            group.Apply(statistic);
            group.Apply(statistic);

            Assert.Single(group.Statistics);
        }        

        [Fact]
        public void Test_Apply_updates_where_group_name_is_the_same()
        {            
            var group = new StatisticGroup("x");
            const string newValue = "123";
            
            group.Apply(new Statistic("x", It.IsAny<string>()));            
            group.Apply(new Statistic("x", newValue));

            Assert.True(group.Statistics.Single().Value == newValue);
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
            var group = new StatisticGroup("x", statistics);

            Assert.True(group.Statistics.SequenceEqual(statistics));
        }

        [Fact]
        public void Test_ChangeStream_pumps_when_statistic_is_added()
        {
            var group = new StatisticGroup("x");
            var pumped = false;
            group.ChangedStream.Subscribe(_ => pumped = true);
            group.Apply(new Statistic("x", string.Empty));    
            Assert.True(pumped);
        }

        [Fact]
        public void Test_ChangeStream_pumps_when_statistic_value_is_changed()
        {
            var group = new StatisticGroup("x");
            var pumped = false;
            group.ChangedStream.Subscribe(_ => pumped = true);
            group.Apply(new Statistic("x", string.Empty));
            group.Apply(new Statistic("x", "1"));
            Assert.True(pumped);
        }

        [Fact]
        public void Test_ChangeStream_does_not_pump_when_value_is_the_same()
        {
            var group = new StatisticGroup("x");
            var pumped = false;
            group.Apply(new Statistic("x", string.Empty));
            group.ChangedStream.Subscribe(_ => pumped = true);
            group.Apply(new Statistic("x", string.Empty));
            Assert.False(pumped);
        }
    }
}
