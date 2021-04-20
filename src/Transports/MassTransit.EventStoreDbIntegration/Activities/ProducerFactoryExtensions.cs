using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Activities;

namespace MassTransit.EventStoreDbIntegration.Activities
{
    static class ProducerFactoryExtensions
    {
        internal static Task<IEventStoreDbProducer> GetProducer<T>(this BehaviorContext<T> context, ConsumeContext consumeContext, StreamName streamName)
        {
            var factory = context.GetStateMachineActivityFactory();

            var rider = factory.GetService<IEventStoreDbRider>(context) ?? throw new ProduceException("EventStoreDbRider not found");

            var producerProvider = rider.GetProducerProvider(consumeContext);

            return producerProvider.GetProducer(streamName);
        }
    }
}
