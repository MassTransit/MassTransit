namespace MassTransit.Azure.ServiceBus.Core.Topology.Specifications
{
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class SubscriptionConsumeTopologySpecification :
        IServiceBusConsumeTopologySpecification
    {
        readonly Filter _filter;
        readonly RuleDescription _rule;
        readonly SubscriptionDescription _subscriptionDescription;
        readonly TopicDescription _topicDescription;

        public SubscriptionConsumeTopologySpecification(TopicDescription topicDescription, SubscriptionDescription subscriptionDescription,
            RuleDescription rule, Filter filter)
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

            subscriptionDescription.ForwardTo = builder.Queue.Queue.QueueDescription.Path;

            builder.CreateQueueSubscription(topic, builder.Queue, subscriptionDescription, _rule, _filter);
        }
    }
}
