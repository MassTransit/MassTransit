namespace MassTransit.Testing.Indicators
{
    using GreenPipes;


    /// <summary>
    /// Represents a boolean condition which may be observed.
    /// </summary>
    public interface IObservableCondition : ICondition
    {
        ConnectHandle ConnectConditionObserver(IConditionObserver observer);
    }
}
