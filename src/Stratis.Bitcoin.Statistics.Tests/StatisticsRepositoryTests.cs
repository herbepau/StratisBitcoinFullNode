using Moq;
using Stratis.Bitcoin.Statistics.Interfaces;
using Xunit;

namespace Stratis.Bitcoin.Statistics.Tests
{
    public class StatisticsRepositoryTests
    {
        private readonly IStatisticsRepository repository;

        public StatisticsRepositoryTests()
        {
            this.repository = new StatisticsRepository();
        }

        [Fact]
        public void Test_Apply_adds_a_category()
        {
            const string name = "x";
            this.repository.Apply(name, new Statistic("stat", It.IsAny<string>()));

            Assert.Single(this.repository.Categories);
        }

        [Fact]
        public void Test_Apply_does_not_add_more_than_one_category_with_same_name()
        {
            const string name = "x";
            this.repository.Apply(name, new Statistic("stat", It.IsAny<string>()));
            this.repository.Apply(name, new Statistic("stat", It.IsAny<string>()));

            Assert.Single(this.repository.Categories);
        }
    }
}
