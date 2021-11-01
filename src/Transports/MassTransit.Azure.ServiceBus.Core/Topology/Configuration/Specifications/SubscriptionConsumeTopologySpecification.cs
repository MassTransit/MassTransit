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
        readonly CreateSubscriptionOptions _createSubscriptionOptions;
        readonly CreateTopicOptions _createTopicOptions;
        readonly RuleFilter _filter;
        readonly CreateRuleOptions _rule;

        public SubscriptionConsumeTopologySpecification(CreateTopicOptions createTopicOptions, CreateSubscriptionOptions createSubscriptionOptions,
            CreateRuleOptions rule, RuleFilter filter)
        {
            _createTopicOptions = createTopicOptions;
            _createSubscriptionOptions = createSubscriptionOptions;
            _rule = rule;
            _filter = filter;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var topic = builder.CreateTopic(_createTopicOptions);

            _createSubscriptionOptions.ForwardTo = builder.Queue.Queue.CreateQueueOptions.Name;

            builder.CreateQueueSubscription(topic, builder.Queue, _createSubscriptionOptions, _rule, _filter);
        }
    }
}
