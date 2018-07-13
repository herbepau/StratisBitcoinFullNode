using Xunit;

namespace Stratis.Bitcoin.Statistics.Tests
{
    public class StatisticsCategoryTests
    {
        [Fact]
        public void Test_CategoryName_is_assigned()
        {
            const string name = "name";
            var category = new StatisticsCategory(name);

            Assert.Equal(name, category.CategoryName);
        }
    }
}
