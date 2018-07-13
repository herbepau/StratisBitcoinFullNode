using System;
using System.Collections.Generic;
using System.Text;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        IStatisticsCategory AddOrUpdate(string categoryName, IEnumerable<IStatistic> statistics);
    }
}
