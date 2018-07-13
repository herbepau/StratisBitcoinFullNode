using System;
using System.Collections.Generic;
using System.Text;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        public IStatisticsCategory AddOrUpdate(string categoryName, IEnumerable<IStatistic> statistics)
        {
            throw new NotImplementedException();
        }
    }
}
