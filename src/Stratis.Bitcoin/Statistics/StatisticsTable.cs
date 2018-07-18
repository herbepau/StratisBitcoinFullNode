using System.Collections.Generic;
using System.Linq;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsTable : IStatisticsTable
    {
        private readonly object @lock = new object();
        private readonly List<IStatisticGroup> rows = new List<IStatisticGroup>();

        public IEnumerable<IStatisticGroup> Rows
        {
            get
            {
                lock (this.@lock)
                    return this.rows;
            }
        }

        public IStatisticGroup Apply(string rowName)
        {
            lock (this.@lock)
            {
                var row = this.rows.FirstOrDefault(x => x.GroupName == rowName);
                if (row == null)
                    this.rows.Add(row = new StatisticGroup(rowName));
                return row;
            }
        }

        public void Clear()
        {
            lock (this.@lock)
                this.rows.Clear();
        }
    }

    public class StatisticsTableFactory : IStatisticsTableFactory
    {
        public IStatisticsTable New() => new StatisticsTable();
    }
}
