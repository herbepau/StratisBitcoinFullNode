using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsTable
    {
        IEnumerable<IStatisticGroup> Rows { get; }

        IStatisticGroup Apply(string rowName);

        void Clear();
    }

    public interface IStatisticsTableFactory
    {
        IStatisticsTable New();
    }
}
