namespace MassTransit.Conductor.Directory
{
    using System;
    using Orchestration;


    public interface IProviderRegistration<TInput, TResult> :
        IProviderRegistration<TResult>
        where TResult : class
        where TInput : class
    {
    }


    public interface IProviderRegistration<TResult> :
        IProviderRegistration
        where TResult : class
    {
    }


    public interface IProviderRegistration
    {
        Type ServiceType { get; }
        Type InputType { get; }
        Uri InputAddress { get; }

        void OnConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator);

        IOrchestrationPlanStep<TResponse> CreateResolutionStep<TResponse>()
            where TResponse : class;
    }
}
