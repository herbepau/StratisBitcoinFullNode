using System;
using Stratis.Bitcoin.Statistics.Interfaces;

namespace Stratis.Bitcoin.Statistics
{
    public class Statistic : IStatistic
    {                
        public Statistic(string id, string value, string displayName = "")
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException($"{nameof(id)} expected");

            this.Id = id;
            this.Value = value ?? throw new ArgumentNullException($"{nameof(value)}");
            this.DisplayName = displayName;
        }

        public string Id { get; }

        public string Value { get; }

        public string DisplayName { get; }
    }
}
