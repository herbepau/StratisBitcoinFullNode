using System;
using System.Collections.Generic;
using System.Text;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsCategory
    {
        string CategoryName { get; }

        IEnumerable<IStatistic> Statistics { get; }

        void AddOrUpdate(IStatistic statistic);
    }
}
