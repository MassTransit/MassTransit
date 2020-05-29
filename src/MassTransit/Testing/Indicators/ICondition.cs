namespace MassTransit.Testing.Indicators
{
    /// <summary>
    /// Represents a boolean condition
    /// </summary>
    public interface ICondition
    {
        bool IsMet { get; }
    }
}
