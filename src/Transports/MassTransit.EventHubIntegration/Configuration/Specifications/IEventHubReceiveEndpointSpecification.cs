namespace MassTransit.EventHubIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transports;


    public interface IEventHubReceiveEndpointSpecification :
        IReceiveEndpointObserverConnector,
        ISpecification
    {
        /// <summary>
        /// EventHub name
        /// </summary>
        string EndpointName { get; }

        IReceiveEndpointControl CreateReceiveEndpoint(IBusInstance busInstance);
    }
}
