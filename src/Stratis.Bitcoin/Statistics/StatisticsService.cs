using System.Collections.Generic;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository repository;

        public StatisticsService(IStatisticsRepository repository) => this.repository = repository;

        public void Apply(string categoryName, IEnumerable<IStatistic> statistics) => this.repository.Apply(categoryName, statistics);

        public IEnumerable<IStatisticGroup> Groups => this.repository.Groups;
    }
}
