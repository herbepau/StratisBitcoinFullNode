using System;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class Statistic : IStatistic
    {                
        public Statistic(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"{nameof(name)} expected");

            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }

        public string Value { get; }
    }
}
