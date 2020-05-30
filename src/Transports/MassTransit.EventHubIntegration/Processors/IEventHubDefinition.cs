namespace MassTransit.EventHubIntegration.Processors
{
    using GreenPipes;
    using Registration;
    using Transports;


    public interface IEventHubDefinition :
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
