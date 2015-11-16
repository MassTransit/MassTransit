namespace MassTransit.TestFramework.Indicators
{
    /// <summary>
    /// Represents a boolean condition which may be observed.
    /// </summary>
    public interface IObservableCondition : ICondition
    {
        void RegisterObserver(IConditionObserver observer);
    }
}