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

        ReceiveEndpoint CreateReceiveEndpoint(IBusInstance busInstance);
    }
}
