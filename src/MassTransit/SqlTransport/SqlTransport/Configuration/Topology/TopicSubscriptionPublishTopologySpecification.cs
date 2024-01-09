#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    /// <summary>
    /// Used to bind an exchange to the sending
    /// </summary>
    public class TopicSubscriptionPublishTopologySpecification :
        SqlTopicSubscriptionConfigurator,
        ISqlPublishTopologySpecification
    {
        public TopicSubscriptionPublishTopologySpecification(string topicName, SqlSubscriptionType subscriptionType = SqlSubscriptionType.All,
            string? routingKey = null)
            : base(topicName, subscriptionType, routingKey)
        {
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.CreateTopic(TopicName);

            if (builder.Topic != null)
                builder.CreateTopicSubscription(builder.Topic, exchangeHandle, SubscriptionType, RoutingKey);
        }
    }
}
