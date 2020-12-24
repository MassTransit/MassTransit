namespace MassTransit.EventHubIntegration
{
    using Riders;


    public interface IEventHubRider :
        IRiderControl
    {
        IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default);
    }
}
