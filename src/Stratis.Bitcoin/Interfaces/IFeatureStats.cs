using System.Collections.Generic;
using System.Text;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Interfaces
{
    public interface IFeatureStats
    {
        void AddFeatureStats(StringBuilder benchLog);

        //IEnumerable<IStatistic> FeatureStatistics { get; }
    }
}
