namespace MassTransit.Azure.ServiceBus.Core.Topology.Specifications
{
    using System.Collections.Generic;
    using Builders;
    using global::Azure.Messaging.ServiceBus.Administration;
    using GreenPipes;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class SubscriptionConsumeTopologySpecification :
        IServiceBusConsumeTopologySpecification
    {
        readonly RuleFilter _filter;
        readonly CreateRuleOptions _rule;
        readonly CreateSubscriptionOptions _subscriptionDescription;
        readonly CreateTopicOptions _topicDescription;

        public SubscriptionConsumeTopologySpecification(CreateTopicOptions topicDescription, CreateSubscriptionOptions subscriptionDescription,
            CreateRuleOptions rule, RuleFilter filter)
        {
            _topicDescription = topicDescription;
            _subscriptionDescription = subscriptionDescription;
            _rule = rule;
            _filter = filter;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var topic = builder.CreateTopic(_topicDescription);

            var subscriptionDescription = _subscriptionDescription;

            subscriptionDescription.ForwardTo = builder.Queue.Queue.QueueDescription.Name;

            builder.CreateQueueSubscription(topic, builder.Queue, subscriptionDescription, _rule, _filter);
        }
    }
}
