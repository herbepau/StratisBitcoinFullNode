using System.Collections.Generic;
using System.Text;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Interfaces
{
    public interface INodeStats
    {
        void AddNodeStats(StringBuilder benchLog);
        
        IEnumerable<IStatistic> NodeStatistics { get; }
    }
}
