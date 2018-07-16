﻿using System.Collections.Generic;

namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatisticsService
    {
        void Apply(string categoryName, IEnumerable<IStatistic> statistics);
    }
}
