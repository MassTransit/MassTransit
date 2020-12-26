namespace MassTransit.EventHubIntegration
{
    using Riders;


    public interface IEventHubRider :
        IRiderControl,
        IEventHubEndpointConnector
    {
        IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default);
    }
}
