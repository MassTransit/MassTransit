namespace MassTransit.Conductor.Orchestration
{
    using System.Threading.Tasks;


    public class NextStepOrchestration<TInput, T, TResult> :
        IOrchestration<TInput, TResult>
        where TInput : class
        where T : class
        where TResult : class
    {
        readonly IOrchestration<T, TResult> _next;
        readonly IOrchestrationStep<TInput, T, TResult> _step;

        public NextStepOrchestration(IOrchestrationStep<TInput, T, TResult> step, IOrchestration<T, TResult> next)
        {
            _step = step;
            _next = next;
        }

        public Task<TResult> Execute(OrchestrationContext<TInput> orchestration)
        {
            return _step.Execute(orchestration, _next);
        }
    }
}
