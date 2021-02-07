namespace MassTransit.Conductor.Orchestration
{
    using System.Threading.Tasks;


    public class LastStepOrchestration<TInput, TResult> :
        IOrchestration<TInput, TResult>
        where TInput : class
        where TResult : class
    {
        readonly IOrchestrationStep<TInput, TResult, TResult> _step;

        public LastStepOrchestration(IOrchestrationStep<TInput, TResult, TResult> step)
        {
            _step = step;
        }

        public Task<TResult> Execute(OrchestrationContext<TInput> orchestration)
        {
            return _step.Execute(orchestration, Cache.LastStep);
        }


        static class Cache
        {
            internal static readonly IOrchestration<TResult, TResult> LastStep = new Response();
        }


        class Response :
            IOrchestration<TResult, TResult>
        {
            public Task<TResult> Execute(OrchestrationContext<TResult> orchestration)
            {
                return Task.FromResult(orchestration.Data);
            }
        }
    }
}
