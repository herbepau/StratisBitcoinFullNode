using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        IStatisticsCategory AddOrUpdate(string categoryName, IEnumerable<IStatistic> statistics);
    }
}
