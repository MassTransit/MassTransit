namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class ServiceBusConsumeTopology :
        ConsumeTopology,
        IServiceBusConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IServiceBusPublishTopology _publishTopology;
        readonly IList<IServiceBusConsumeTopologySpecification> _specifications;

        public ServiceBusConsumeTopology(IMessageTopology messageTopology, IServiceBusPublishTopology publishTopology)
            : base(260)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            _specifications = new List<IServiceBusConsumeTopologySpecification>();
        }

        IServiceBusMessageConsumeTopology<T> IServiceBusConsumeTopology.GetMessageTopology<T>()
        {
            return (IServiceBusMessageConsumeTopologyConfigurator<T>)GetMessageTopology<T>();
        }

        IServiceBusMessageConsumeTopologyConfigurator<T> IServiceBusConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return (IServiceBusMessageConsumeTopologyConfigurator<T>)GetMessageTopology<T>();
        }

        public void AddSpecification(IServiceBusConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        public void Subscribe(string topicName, string subscriptionName, Action<IServiceBusSubscriptionConfigurator> callback = null)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topicName));

            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(subscriptionName));

            subscriptionName = _publishTopology.FormatSubscriptionName(subscriptionName);

            var createTopicOptions = Defaults.GetCreateTopicOptions(topicName);

            var subscriptionConfigurator = new ServiceBusSubscriptionConfigurator(subscriptionName, createTopicOptions.Name);

            callback?.Invoke(subscriptionConfigurator);

            var specification = new SubscriptionConsumeTopologySpecification(createTopicOptions, subscriptionConfigurator.GetCreateSubscriptionOptions(),
                subscriptionConfigurator.Rule,
                subscriptionConfigurator.Filter);

            _specifications.Add(specification);
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IServiceBusMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
