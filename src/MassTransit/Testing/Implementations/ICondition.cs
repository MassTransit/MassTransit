namespace MassTransit.Testing.Implementations
{
    /// <summary>
    /// Represents a boolean condition
    /// </summary>
    public interface ICondition
    {
        bool IsMet { get; }
    }
}
