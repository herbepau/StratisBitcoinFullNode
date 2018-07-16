using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticsCategory : IStatisticsCategory
    {
        private readonly object @lock = new object();
        private readonly List<IStatistic> statistics = new List<IStatistic>();
        private readonly Subject<IStatisticsCategory> changedStream = new Subject<IStatisticsCategory>();

        public StatisticsCategory(string categoryName, IEnumerable<IStatistic> statistics = null)
        {
            if (string.IsNullOrEmpty(categoryName))
                throw new ArgumentException($"{nameof(categoryName)} expected");

            this.CategoryName = categoryName;
            this.ChangedStream = this.changedStream.AsObservable();

            if (statistics != null)
                this.statistics.AddRange(statistics);
        } 

        public string CategoryName { get; }
        public IObservable<IStatisticsCategory> ChangedStream { get; }

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
            var changed = false;

            lock (this.@lock)
            {
                foreach (var stat in stats)
                {
                    var index = this.statistics.FindIndex(x => x.Name == stat.Name);
                    if (index == -1)
                    {
                        changed = true;
                        this.statistics.Add(stat);
                    }
                    else if (this.statistics[index].Value != stat.Value)
                    {
                        changed = true;
                        this.statistics[index].Value = stat.Value;
                    }
                }
            }

            if (changed)
                this.changedStream.OnNext(this);
        }
    }
}
