using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatistic
    {
        string Name { get; }

        string Value { get; }

        IEnumerable<IStatistic> Statistics { get; }
    }
}
