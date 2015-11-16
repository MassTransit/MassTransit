namespace MassTransit.TestFramework.Indicators
{
    /// <summary>
    /// Represents an observer on a change in boolean condition state.
    /// </summary>
    public interface IConditionObserver
    {
        void ConditionUpdated();
    }
}