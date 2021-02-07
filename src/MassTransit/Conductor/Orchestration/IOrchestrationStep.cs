namespace MassTransit.Conductor.Orchestration
{
    using System.Threading.Tasks;


    public interface IOrchestrationStep<in TInput, out T, TResult>
        where TInput : class
        where T : class
        where TResult : class
    {
        Task<TResult> Execute(OrchestrationContext<TInput> context, IOrchestration<T, TResult> next);
    }
}
