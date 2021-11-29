namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class ServiceBusMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IServiceBusMessageConsumeTopologyConfigurator<TMessage>,
        IServiceBusMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IServiceBusMessagePublishTopology<TMessage> _publishTopology;
        readonly IList<IServiceBusConsumeTopologySpecification> _specifications;

        public ServiceBusMessageConsumeTopology(IMessageTopology<TMessage> messageTopology, IServiceBusMessagePublishTopology<TMessage> publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;

            _specifications = new List<IServiceBusConsumeTopologySpecification>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Subscribe(string subscriptionName, Action<IServiceBusSubscriptionConfigurator> configure = null)
        {
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(subscriptionName));

            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidServiceBusConsumeTopologySpecification(TypeCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var createTopicOptions = _publishTopology.CreateTopicOptions;

            var subscriptionConfigurator = _publishTopology.GetSubscriptionConfigurator(subscriptionName);

            configure?.Invoke(subscriptionConfigurator);

            var specification = new SubscriptionConsumeTopologySpecification(createTopicOptions, subscriptionConfigurator.GetCreateSubscriptionOptions(),
                subscriptionConfigurator.Rule,
                subscriptionConfigurator.Filter);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
