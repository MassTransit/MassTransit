namespace MassTransit.Conductor
{
    using System.Threading.Tasks;


    public interface IOrchestration<in TInput, TResult>
        where TResult : class
        where TInput : class
    {
        Task<TResult> Execute(OrchestrationContext<TInput> orchestration);
    }
}
