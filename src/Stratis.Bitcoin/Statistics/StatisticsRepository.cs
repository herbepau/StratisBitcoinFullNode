using System.Collections.Concurrent;
using System.Collections.Generic;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ConcurrentDictionary<string, IStatisticGroup> groups = new ConcurrentDictionary<string, IStatisticGroup>();

        public IEnumerable<IStatisticGroup> Groups => this.groups.Values;

        public bool AddGroup(IStatisticGroup group) => this.groups.TryAdd(group.GroupName, group);

        public IStatisticGroup Apply(string groupName, IStatistic statistic) => this.Apply(groupName, new[] { statistic });
        public IStatisticGroup Apply(string groupName, IEnumerable<IStatistic> statistics)
        {
            if (this.groups.TryGetValue(groupName, out IStatisticGroup group))
                group.Apply(statistics);
            else
                this.groups.TryAdd(groupName, group = new StatisticGroup(groupName, statistics));

            return group;
        }
    }
}
