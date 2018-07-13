using System.Collections.Generic;
using System.Linq;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class Statistic : IStatistic
    {
        private readonly List<IStatistic> statistics;

        public Statistic(string name, string value, IEnumerable<IStatistic> statistics = null)
        {
            this.Name = name;
            this.Value = value;
            if (statistics != null)
                this.statistics = this.statistics.ToList();
        }

        public string Name { get; }

        public string Value { get; }

        public IEnumerable<IStatistic> Statistics => this.statistics;
    }
}
