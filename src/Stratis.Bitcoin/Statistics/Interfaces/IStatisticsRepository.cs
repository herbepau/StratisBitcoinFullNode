using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsRepository
    {
        IEnumerable<IStatisticGroup> Groups { get; }

        void Apply(string groupName, IStatistic statistic);

        void Apply(string groupName, IEnumerable<IStatistic> statistics);
    }
}
