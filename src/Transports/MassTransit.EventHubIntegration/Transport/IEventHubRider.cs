namespace MassTransit.EventHubIntegration
{
    using Riders;


    public interface IEventHubRider :
        IRiderControl,
        IEvenHubEndpointConnector
    {
        IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default);
    }
}
