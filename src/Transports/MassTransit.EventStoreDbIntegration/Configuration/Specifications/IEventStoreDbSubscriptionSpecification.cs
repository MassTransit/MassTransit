using GreenPipes;
using MassTransit.Registration;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public interface IEventStoreDbSubscriptionSpecification :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        /// <summary>
        /// EventStoreDB subscription endpoint name.
        /// </summary>
        string EndpointName { get; }

        ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance);
    }
}
