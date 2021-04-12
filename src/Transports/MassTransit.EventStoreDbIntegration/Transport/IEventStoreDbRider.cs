using MassTransit.Riders;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbRider :
        IRiderControl
    {
        IEventStoreDbProducerProvider GetProducerProvider(ConsumeContext consumeContext = default);
    }
}
