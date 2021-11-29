namespace MassTransit.EventHubIntegration.Configuration
{
    using Transports;


    public interface IEventHubReceiveEndpointSpecification :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        /// <summary>
        /// EventHub name
        /// </summary>
        string EndpointName { get; }

        ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance);
    }
}
