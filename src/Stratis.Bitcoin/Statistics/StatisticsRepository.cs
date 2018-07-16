using System.Collections.Concurrent;
using System.Collections.Generic;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ConcurrentDictionary<string, IStatisticsCategory> categories = new ConcurrentDictionary<string, IStatisticsCategory>();

        public IEnumerable<IStatisticsCategory> Categories => this.categories.Values;

        public void Apply(string categoryName, IStatistic statistic)
        {
            this.Apply(categoryName, new[] {statistic});
        }

        public void Apply(string categoryName, IEnumerable<IStatistic> statistics)
        {
            if (this.categories.TryGetValue(categoryName, out IStatisticsCategory category))
            {
                category.Apply(statistics);
                return;
            }

            this.categories.TryAdd(categoryName, new StatisticsCategory(categoryName, statistics));
        }
    }
}
