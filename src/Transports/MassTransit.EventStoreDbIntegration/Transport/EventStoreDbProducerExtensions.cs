using System;
using System.Threading.Tasks;

namespace MassTransit.EventStoreDbIntegration
{
    public static class EventStoreDbProducerExtensions
    {
        public static Task<IEventStoreDbProducer> GetProducer(this IEventStoreDbProducerProvider producerProvider, string streamName)
        {
            if (producerProvider == null)
                throw new ArgumentNullException(nameof(producerProvider));
            if (string.IsNullOrWhiteSpace(streamName))
                throw new ArgumentNullException(nameof(streamName));

            return producerProvider.GetProducer(new Uri($"queue:{streamName}"));
        }
    }
}
