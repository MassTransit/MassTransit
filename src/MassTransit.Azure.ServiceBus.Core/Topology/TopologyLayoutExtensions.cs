namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using Context;


    public static class TopologyLayoutExtensions
    {
        public static void LogResult(this BrokerTopology topology)
        {
            foreach (var topic in topology.Topics)
            {
                LogContext.Info?.Log("Topic: {Topic}", topic.TopicDescription.Path);
            }

            foreach (var subscription in topology.Subscriptions)
            {
                LogContext.Info?.Log("Subscription: {Subscription}, topic: {Topic}", subscription.SubscriptionDescription.SubscriptionName,
                    subscription.SubscriptionDescription.TopicPath);
            }
        }
    }
}
