namespace MassTransit.Conductor.Orchestration
{
    using System.Threading.Tasks;
    using Context;
    using GreenPipes.Internals.Extensions;


    public class FactoryOrchestrationStep<TInput, T, TResult> :
        IOrchestrationStep<TInput, T, TResult>
        where TInput : class
        where TResult : class
        where T : class
    {
        readonly AsyncMessageFactory<TInput, T> _factory;

        public FactoryOrchestrationStep(AsyncMessageFactory<TInput, T> factory)
        {
            _factory = factory;
        }

        public async Task<TResult> Execute(OrchestrationContext<TInput> context, IOrchestration<T, TResult> next)
        {
            LogContext.Debug?.Log("Factory<{RequestType}, {ResultType}", TypeCache<TInput>.ShortName, TypeCache<T>.ShortName);

            var value = await _factory(context).ConfigureAwait(false);

            OrchestrationContext<T> nextContext = context.Push(value);

            return await next.Execute(nextContext).ConfigureAwait(false);
        }
    }
}
