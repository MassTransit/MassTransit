namespace MassTransit.Configuration
{
    using System;


    public interface IJobServiceRegistration :
        IRegistration
    {
        IEndpointRegistrationConfigurator EndpointRegistrationConfigurator { get; }

        IEndpointDefinition EndpointDefinition { get; }

        void AddConfigureAction(Action<JobConsumerOptions> configure);

        void AddReceiveEndpointDependency(IReceiveEndpointConfigurator dependency);

        void Configure(IServiceInstanceConfigurator instanceConfigurator, IRegistrationContext context);
    }
}
