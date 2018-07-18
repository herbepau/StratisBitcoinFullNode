using System.Collections.Generic;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository repository;

        public StatisticsService(IStatisticsRepository repository, IStatisticsTableFactory tableFactory)
        {
            this.repository = repository;
            this.LightWalletStatistics = tableFactory.New();
            this.WalletStatistics = tableFactory.New();
        }

        public IEnumerable<IStatisticGroup> Groups => this.repository.Groups;
        public IStatisticsTable LightWalletStatistics { get; }
        public IStatisticsTable WalletStatistics { get; }

        public void Apply(string categoryName, IStatistic statistic) => this.repository.Apply(categoryName, statistic);

        public void Apply(string categoryName, IEnumerable<IStatistic> statistics) => this.repository.Apply(categoryName, statistics);
    }
}
