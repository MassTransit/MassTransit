namespace MassTransit
{
    using AzureServiceBusTransport.Topology;


    public static class ServiceBusBrokerTopologyExtensions
    {
        public static void LogResult(this BrokerTopology topology)
        {
            foreach (var topic in topology.Topics)
                LogContext.Info?.Log("Topic: {Topic}", topic.CreateTopicOptions.Name);

            foreach (var subscription in topology.Subscriptions)
            {
                LogContext.Info?.Log("Subscription: {Subscription}, topic: {Topic}", subscription.CreateSubscriptionOptions.SubscriptionName,
                    subscription.CreateSubscriptionOptions.TopicName);
            }
        }
    }
}
