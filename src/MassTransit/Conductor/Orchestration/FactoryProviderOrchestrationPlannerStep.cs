namespace MassTransit.Conductor.Orchestration
{
    using System;
    using GreenPipes.Internals.Extensions;


    public class FactoryProviderOrchestrationPlannerStep<TInput, T, TResult> :
        IOrchestrationPlannerStep<TInput, T, TResult>
        where TInput : class
        where T : class
        where TResult : class
    {
        readonly AsyncMessageFactory<TInput, T> _factory;

        public FactoryProviderOrchestrationPlannerStep(Uri inputAddress, AsyncMessageFactory<TInput, T> factory)
        {
            InputAddress = inputAddress;
            _factory = factory;
        }

        public Type ServiceType => typeof(T);
        public Type RequestType => typeof(TInput);
        public Uri InputAddress { get; }

        public void Build(BuildContext<TInput, TResult> context, IOrchestrationPlan<T, TResult> next)
        {
            NextBuildContext<T, TResult, TInput> nextContext = context.Create<T>();
            next.Build(nextContext);

            context.Factory(nextContext, _factory);
        }

        public override string ToString()
        {
            return $"Factory: {TypeCache<TInput>.ShortName} -> {TypeCache<T>.ShortName}";
        }
    }
}
