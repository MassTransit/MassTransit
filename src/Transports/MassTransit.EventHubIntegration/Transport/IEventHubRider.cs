namespace MassTransit.EventHubIntegration
{
    using Riders;


    public interface IEventHubRider :
        IRider
    {
        IProducerProvider GetProducerProvider(ConsumeContext consumeContext = default);
    }
}
