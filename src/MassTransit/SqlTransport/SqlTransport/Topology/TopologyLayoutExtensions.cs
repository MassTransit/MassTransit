namespace MassTransit.SqlTransport.Topology
{
    public static class TopologyLayoutExtensions
    {
        public static void LogResult(this BrokerTopology layout)
        {
            foreach (var topic in layout.Topics)
                LogContext.Info?.Log("Topic: {TopicName}", topic.TopicName);

            foreach (var subscription in layout.TopicSubscriptions)
            {
                LogContext.Info?.Log("Topic Subscription: source {Source}, destination: {Destination}, subscriptionType: {Type} routingKey: {RoutingKey}",
                    subscription.Source.TopicName, subscription.Destination.TopicName, subscription.SubscriptionType, subscription.RoutingKey);
            }

            foreach (var queue in layout.Queues)
                LogContext.Info?.Log("Queue: {QueueName}, auto-delete: {AutoDeleteOnIdle}", queue.QueueName, queue.AutoDeleteOnIdle);

            foreach (var subscription in layout.QueueSubscriptions)
            {
                LogContext.Info?.Log("Queue Subscription: source {Source}, destination: {Destination}, subscriptionType: {Type} routingKey: {RoutingKey}",
                    subscription.Source.TopicName, subscription.Destination.QueueName, subscription.SubscriptionType, subscription.RoutingKey);
            }
        }
    }
}
