namespace MassTransit.Conductor
{
    public interface IOrchestrationProvider
    {
        IOrchestration<TInput, TResult> GetOrchestration<TInput, TResult>()
            where TInput : class
            where TResult : class;
    }
}
