namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using Transports;


    public interface IEventHubHostConfiguration :
        ISpecification
    {
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        IEventHubReceiveEndpointSpecification CreateSpecification(string eventHubName, string consumerGroup,
            Action<IEventHubReceiveEndpointConfigurator> configure);

        IEventHubRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
    }
}
