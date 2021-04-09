using MassTransit.Riders;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbRider :
        IRiderControl,
        IEventStoreDbEndpointConnector
    {
        IEventStoreDbProducerProvider GetProducerProvider(ConsumeContext consumeContext = default);
    }
}
