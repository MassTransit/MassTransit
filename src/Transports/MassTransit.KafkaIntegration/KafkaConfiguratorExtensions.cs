namespace MassTransit.KafkaIntegration
{
    using System;
    using Subscriptions;


    public static class KafkaConfiguratorExtensions
    {
        public static void Subscribe<TKey, TValue>(this IKafkaFactoryConfigurator configurator, string topic,
            Action<IKafkaSubscriptionConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            configurator.Subscribe(new DefaultTopicNameFormatter(topic), configure);
        }
    }
}
