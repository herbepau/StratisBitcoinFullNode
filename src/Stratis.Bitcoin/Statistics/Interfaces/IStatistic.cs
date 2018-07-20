namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatistic
    {
        string Id { get; }

        string DisplayName { get; }

        string Value { get; }
    }
}
