using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        void Apply(string groupName, IStatistic statistic);

        void Apply(string groupName, IEnumerable<IStatistic> statistics);

        IEnumerable<IStatisticGroup> Groups { get; }

        IStatisticsTable LightWalletStatistics { get; }

        IStatisticsTable WalletStatistics { get; }
    }
}
