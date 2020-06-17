namespace MassTransit.EventHubIntegration
{
    using System;


    public static class EventHubProducerExtensions
    {
        public static IEventHubProducer GetProducer(this IEventHubProducerProvider producerProvider, string eventHubName)
        {
            if (producerProvider == null)
                throw new ArgumentNullException(nameof(producerProvider));
            if (string.IsNullOrWhiteSpace(eventHubName))
                throw new ArgumentNullException(nameof(eventHubName));

            return producerProvider.GetProducer(new Uri($"topic:{eventHubName}"));
        }
    }
}
