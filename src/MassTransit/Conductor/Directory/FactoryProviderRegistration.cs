namespace MassTransit.Conductor.Directory
{
    using System;
    using Orchestration;


    public class FactoryProviderRegistration<TInput, TResult> :
        IProviderRegistration<TInput, TResult>
        where TResult : class
        where TInput : class
    {
        readonly AsyncMessageFactory<TInput, TResult> _factory;

        public FactoryProviderRegistration(AsyncMessageFactory<TInput, TResult> factory)
        {
            _factory = factory;
        }

        public Type ServiceType => typeof(TResult);
        public Type InputType => typeof(TInput);

        public Uri InputAddress => default;

        public void OnConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
        }

        public IOrchestrationPlanStep<TResponse> CreateResolutionStep<TResponse>()
            where TResponse : class
        {
            return new FactoryProviderOrchestrationPlannerStep<TInput, TResult, TResponse>(InputAddress, _factory);
        }
    }
}
