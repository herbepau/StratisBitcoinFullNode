using System.Collections.Concurrent;
using System.Collections.Generic;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsRepository : IStatisticsRepository
    {        
        private readonly ConcurrentDictionary<string, IStatisticGroup> groups = new ConcurrentDictionary<string, IStatisticGroup>();        

        public IEnumerable<IStatisticGroup> Groups => this.groups.Values;

        public void Apply(string groupName, IStatistic statistic)
        {
            this.Apply(groupName, new[] {statistic});
        }

        public void Apply(string groupName, IEnumerable<IStatistic> statistics)
        {
            if (this.groups.TryGetValue(groupName, out IStatisticGroup group))
                group.Apply(statistics);
            else
                this.groups.TryAdd(groupName, new StatisticGroup(groupName, statistics));
        }
    }
}
