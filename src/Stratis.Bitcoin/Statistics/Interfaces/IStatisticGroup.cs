using System;
using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticGroup
    {        
        string GroupName { get; }               

        string ChangedTimeFormatted { get; }

        IEnumerable<IStatistic> Statistics { get; }

        IObservable<IStatisticGroup> ChangedStream { get; }

        void Apply(IStatistic statistic);

        void Apply(IEnumerable<IStatistic> statistics);
    }
}
