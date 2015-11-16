namespace MassTransit.TestFramework.Indicators
{
    /// <summary>
    /// Represents a boolean condition
    /// </summary>
    public interface ICondition
    {
        bool State { get; }
    }
}