using System;
using System.Collections.Concurrent;
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
        private readonly object lockObject = new object();
        private readonly List<IStatistic> statistics = new List<IStatistic>();
        private readonly ConcurrentBag<IStatisticGroup> groups = new ConcurrentBag<IStatisticGroup>();
        private readonly Subject<IStatisticGroup> changedStream = new Subject<IStatisticGroup>();

        public StatisticGroup(string groupName, IEnumerable<IStatistic> statistics = null)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentException($"{nameof(groupName)} expected");

            this.GroupName = groupName;
            this.ChangedStream = this.changedStream.AsObservable();

            if (statistics != null)
                this.statistics.AddRange(statistics);
        }

        [JsonIgnore]
        public IObservable<IStatisticGroup> ChangedStream { get; }
        public string GroupName { get; }
        public IStatistic this[string statisticId] => this.Statistics.FirstOrDefault(x => x.Id == statisticId);
        public IEnumerable<IStatisticGroup> Groups => this.groups.ToList();

        public IEnumerable<IStatistic> Statistics
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.statistics.ToList();
                }
            }
        }

        public IStatisticGroup AddGroup(string name)
        {
            var group = this.groups.FirstOrDefault(x => x.GroupName == name);
            if (group == null)
                this.groups.Add(group = new StatisticGroup(name));
            return group;
        }

        public void Apply(IStatistic statistic)
        {
            this.Apply(new []{ statistic });
        }

        public void Apply(IEnumerable<IStatistic> stats)
        {
            var changed = false;

            lock (this.lockObject)
            {
                foreach (var stat in stats)
                {
                    var index = this.statistics.FindIndex(x => x.Id == stat.Id);
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
    }
}
