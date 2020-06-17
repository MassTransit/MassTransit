namespace MassTransit.EventHubIntegration
{
    using Riders;


    public interface IEventHubRider :
        IRider
    {
        IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default);
    }
}
