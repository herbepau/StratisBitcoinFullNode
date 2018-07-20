using System.Collections.Generic;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository repository;

        public StatisticsService(IStatisticsRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<IStatisticGroup> Groups => this.repository.Groups;

        public bool AddGroup(IStatisticGroup group) => this.repository.AddGroup(group);

        public IStatisticGroup Apply(string categoryName, IStatistic statistic) => this.repository.Apply(categoryName, statistic);
        public IStatisticGroup Apply(string categoryName, IEnumerable<IStatistic> statistics) => this.repository.Apply(categoryName, statistics);
    }
}
