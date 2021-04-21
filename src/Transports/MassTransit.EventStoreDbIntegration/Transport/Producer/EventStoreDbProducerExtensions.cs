using System;
using System.Threading.Tasks;

namespace MassTransit.EventStoreDbIntegration
{
    public static class EventStoreDbProducerExtensions
    {
        public static Task<IEventStoreDbProducer> GetProducer(this IEventStoreDbProducerProvider producerProvider, StreamName streamName)
        {
            if (producerProvider == null)
                throw new ArgumentNullException(nameof(producerProvider));
            if (streamName == null)
                throw new ArgumentNullException(nameof(streamName));

            return producerProvider.GetProducer(new Uri($"stream:{streamName}"));
        }
    }
}
