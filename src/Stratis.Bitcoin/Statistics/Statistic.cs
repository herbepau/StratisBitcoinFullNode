using System.Collections.Generic;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class Statistic : IStatistic
    {
        private readonly List<IStatistic> statistics = new List<IStatistic>();

        public Statistic(string name, string value, IEnumerable<IStatistic> statistics = null)
        {
            this.Name = name;
            this.Value = value;

            if (statistics != null)
                this.statistics.AddRange(statistics);
        }

        public string Name { get; }

        public string Value { get; }

        public IEnumerable<IStatistic> Statistics => this.statistics;
    }
}
