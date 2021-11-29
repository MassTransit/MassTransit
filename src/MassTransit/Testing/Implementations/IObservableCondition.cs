namespace MassTransit.Testing.Implementations
{
    /// <summary>
    /// Represents a boolean condition which may be observed.
    /// </summary>
    public interface IObservableCondition : ICondition
    {
        ConnectHandle ConnectConditionObserver(IConditionObserver observer);
    }
}
