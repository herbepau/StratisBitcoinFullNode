using System;
using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticGroup
    {        
        IStatistic this[string statisticId] { get; }

        string GroupName { get; }

        IEnumerable<IStatistic> Statistics { get; }

        IEnumerable<IStatisticGroup> Groups { get; }

        IObservable<IStatisticGroup> ChangedStream { get; }

        void Apply(IStatistic statistic);

        void Apply(IEnumerable<IStatistic> statistics);

        IStatisticGroup AddGroup(string name);
    }
}
