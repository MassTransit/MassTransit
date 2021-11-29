namespace MassTransit.Testing.Implementations
{
    using System.Threading.Tasks;


    /// <summary>
    /// Represents an observer on a change in boolean condition state.
    /// </summary>
    public interface IConditionObserver
    {
        Task ConditionUpdated();
    }
}
