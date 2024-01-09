#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    public abstract class SqlTopicSubscriptionConfigurator :
        SqlTopicConfigurator,
        ISqlTopicSubscriptionConfigurator
    {
        protected SqlTopicSubscriptionConfigurator(string topicName, SqlSubscriptionType subscriptionType = SqlSubscriptionType.All, string? routingKey = null)
            : base(topicName)
        {
            SubscriptionType = subscriptionType;
            RoutingKey = routingKey;
        }

        public string? RoutingKey { get; set; }
        public SqlSubscriptionType SubscriptionType { get; set; }
    }
}
