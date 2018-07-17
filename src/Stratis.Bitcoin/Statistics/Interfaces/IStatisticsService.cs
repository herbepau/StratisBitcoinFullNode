using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        void Apply(string groupName, IEnumerable<IStatistic> statistics);

        IEnumerable<IStatisticGroup> Groups { get; }
    }
}
