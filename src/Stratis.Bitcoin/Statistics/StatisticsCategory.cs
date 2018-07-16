using System.Collections.Generic;
using System.Linq;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsCategory : IStatisticsCategory
    {
        private readonly object @lock = new object();
        private readonly List<IStatistic> statistics = new List<IStatistic>();

        public StatisticsCategory(string categoryName, IEnumerable<IStatistic> statistics = null)
        {
            this.CategoryName = categoryName;

            if (statistics != null)
                this.statistics.AddRange(statistics);
        } 

        public string CategoryName { get; }

        public IEnumerable<IStatistic> Statistics
        {
            get
            {
                lock (this.@lock)
                    return this.statistics.ToList();
            }
        }

        public void Apply(IStatistic statistic)
        {
            this.Apply(new []{ statistic });
        }

        public void Apply(IEnumerable<IStatistic> stats)
        {
            lock (this.@lock)
            {
                foreach (var stat in stats)
                {
                    var storedStat = this.statistics.FirstOrDefault(x => x.Name == stat.Name);
                    if (storedStat != null)
                        this.statistics[this.statistics.IndexOf(storedStat)] = stat;
                    else
                        this.statistics.Add(stat);
                }
            }
        }
    }
}
