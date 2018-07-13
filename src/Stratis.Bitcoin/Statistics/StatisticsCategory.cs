using System.Collections.Generic;
using System.Linq;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsCategory : IStatisticsCategory
    {
        private readonly object @lock = new object();
        private readonly List<IStatistic> statistics = new List<IStatistic>();

        public StatisticsCategory(string categoryName) => this.CategoryName = categoryName;        

        public string CategoryName { get; }

        public IEnumerable<IStatistic> Statistics
        {
            get
            {
                lock (this.@lock)
                    return this.statistics.ToList();
            }
        }

        public void AddOrUpdate(IStatistic statistic)
        {            
            lock (this.@lock)
            {
                var stat = this.statistics.FirstOrDefault(x => x.Name == statistic.Name);
                if (stat != null)
                    this.statistics[this.statistics.IndexOf(stat)] = statistic;
                else
                    this.statistics.Add(statistic);
            }
        }
    }
}
