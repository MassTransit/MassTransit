namespace MassTransit.EventHubIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;
    using Transports;


    public interface IEventHubSpecification :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        /// <summary>
        /// EventHub name
        /// </summary>
        string Name { get; }

        IEventHubReceiveEndpoint Create(IBusInstance busInstance);
    }
}
