using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        bool AddGroup(IStatisticGroup group);

        IStatisticGroup Apply(string groupName, IStatistic statistic);

        IStatisticGroup Apply(string groupName, IEnumerable<IStatistic> statistics);

        IEnumerable<IStatisticGroup> Groups { get; }
    }
}
