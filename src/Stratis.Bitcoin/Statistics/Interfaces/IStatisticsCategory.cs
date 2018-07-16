using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsCategory
    {
        string CategoryName { get; }

        IEnumerable<IStatistic> Statistics { get; }

        void Apply(IStatistic statistic);

        void Apply(IEnumerable<IStatistic> statistics);
    }
}
