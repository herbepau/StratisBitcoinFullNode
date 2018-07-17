using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class StatisticGroup : IStatisticGroup
    {
        private readonly object @lock = new object();
        private readonly List<IStatistic> statistics = new List<IStatistic>();
        private readonly Subject<IStatisticGroup> changedStream = new Subject<IStatisticGroup>();

        public StatisticGroup(string groupName, IEnumerable<IStatistic> statistics = null)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentException($"{nameof(groupName)} expected");

            this.changedStream.StartWith(this).Subscribe(x => this.SetChangedTimeFormatted());

            this.GroupName = groupName;
            this.ChangedStream = this.changedStream.AsObservable();

            if (statistics != null)
                this.Apply(statistics);

            this.SetChangedTimeFormatted();
        } 

        public string GroupName { get; }
        public string ChangedTimeFormatted { get; private set; }

        [JsonIgnore]
        public IObservable<IStatisticGroup> ChangedStream { get; }

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
                        this.statistics[index] = stat;
                    }
                }
            }

            if (!changed)
                return;

            this.changedStream.OnNext(this);
        }

        private void SetChangedTimeFormatted() => this.ChangedTimeFormatted = DateTime.Now.ToString("HHmmss");
    }
}
