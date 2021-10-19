namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using GreenPipes;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Specifications;


    public class AmazonSqsMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IAmazonSqsMessageConsumeTopologyConfigurator<TMessage>,
        IAmazonSqsMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IAmazonSqsMessagePublishTopology<TMessage> _messagePublishTopology;
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IAmazonSqsPublishTopology _publishTopology;
        readonly IList<IAmazonSqsConsumeTopologySpecification> _specifications;

        public AmazonSqsMessageConsumeTopology(IMessageTopology<TMessage> messageTopology, IAmazonSqsPublishTopology publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            _messagePublishTopology = _publishTopology.GetMessageTopology<TMessage>();

            _specifications = new List<IAmazonSqsConsumeTopologySpecification>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Subscribe(Action<ITopicSubscriptionConfigurator> configure = null)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidAmazonSqsConsumeTopologySpecification(TypeMetadataCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var specification = new ConsumerConsumeTopologySpecification(_publishTopology, _messagePublishTopology.Topic);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
