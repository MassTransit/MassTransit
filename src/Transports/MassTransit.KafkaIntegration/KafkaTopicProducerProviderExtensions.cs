namespace MassTransit
{
    using System;
    using Confluent.Kafka;
    using DependencyInjection;
    using KafkaIntegration;
    using Microsoft.Extensions.DependencyInjection;


    public static class KafkaTopicProducerProviderExtensions
    {
        public static ITopicProducer<TValue> GetProducer<TValue>(this ITopicProducerProvider provider, Uri address)
            where TValue : class
        {
            return GetProducer<Null, TValue>(provider, address, context => default);
        }

        public static ITopicProducer<TValue> GetProducer<TKey, TValue>(this ITopicProducerProvider provider, Uri address,
            KafkaKeyResolver<TKey, TValue> keyResolver)
            where TValue : class
        {
            ITopicProducer<TKey, TValue> producer = provider.GetProducer<TKey, TValue>(address);
            return new KeyedTopicProducer<TKey, TValue>(producer, keyResolver);
        }

        internal static ITopicProducerProvider GetScopedTopicProducerProvider(this ITopicProducerProvider producerProvider, IServiceProvider provider)
        {
            var contextProvider = provider.GetService<IScopedConsumeContextProvider>();
            return contextProvider is { HasContext: true }
                ? new ConsumeContextTopicProducerProvider(producerProvider, contextProvider.GetContext())
                : producerProvider;
        }
    }
}
