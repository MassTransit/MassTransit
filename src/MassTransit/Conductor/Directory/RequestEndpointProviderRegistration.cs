namespace MassTransit.Conductor.Directory
{
    using System;
    using Orchestration;


    public class RequestEndpointProviderRegistration<TInput, TResult> :
        IProviderRegistration<TInput, TResult>
        where TResult : class
        where TInput : class
    {
        IReceiveEndpointConfigurator _receiveEndpointConfigurator;

        public Type ServiceType => typeof(TResult);
        public Type InputType => typeof(TInput);
        public Uri InputAddress => _receiveEndpointConfigurator?.InputAddress;

        public void OnConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _receiveEndpointConfigurator = configurator;
        }

        public IOrchestrationPlanStep<TResponse> CreateResolutionStep<TResponse>()
            where TResponse : class
        {
            return new RequestClientOrchestrationPlannerStep<TInput, TResult, TResponse>(InputAddress);
        }
    }
}
