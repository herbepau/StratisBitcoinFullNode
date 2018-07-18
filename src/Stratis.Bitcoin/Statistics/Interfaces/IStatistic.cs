namespace Stratis.Bitcoin.Statistics.Interfaces
{
    public interface IStatistic
    {
        string Name { get; }

        string Value { get; }
    }
}
