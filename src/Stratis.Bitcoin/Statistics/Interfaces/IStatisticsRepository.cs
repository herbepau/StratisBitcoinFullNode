using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsRepository
    {
        IEnumerable<IStatisticsCategory> Categories { get; }

        void Apply(string categoryName, IStatistic statistic);

        void Apply(string categoryName, IEnumerable<IStatistic> statistics);
    }
}
