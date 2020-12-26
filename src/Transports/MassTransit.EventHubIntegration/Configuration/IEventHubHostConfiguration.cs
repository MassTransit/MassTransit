namespace MassTransit.EventHubIntegration
{
    using System;
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;
    using Specifications;


    public interface IEventHubHostConfiguration :
        ISpecification
    {
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        IEventHubReceiveEndpointSpecification CreateSpecification(string eventHubName, string consumerGroup,
            Action<IEventHubReceiveEndpointConfigurator> configure);

        IEventHubRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
    }
}
