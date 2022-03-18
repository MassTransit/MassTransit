namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus.Administration;
    using Topology;


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
            _createSubscriptionOptions.AutoDeleteOnIdle = builder.Queue.Queue.CreateQueueOptions.AutoDeleteOnIdle;

            builder.CreateQueueSubscription(topic, builder.Queue, _createSubscriptionOptions, _rule, _filter);
        }
    }
}
