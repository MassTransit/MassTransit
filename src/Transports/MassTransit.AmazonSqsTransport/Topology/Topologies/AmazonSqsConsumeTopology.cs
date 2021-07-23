namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using GreenPipes;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Specifications;


    public class AmazonSqsConsumeTopology :
        ConsumeTopology,
        IAmazonSqsConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IAmazonSqsPublishTopology _publishTopology;
        readonly IList<IAmazonSqsConsumeTopologySpecification> _specifications;

        public AmazonSqsConsumeTopology(IMessageTopology messageTopology, IAmazonSqsPublishTopology publishTopology)
            : base(72)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;

            _specifications = new List<IAmazonSqsConsumeTopologySpecification>();
        }

        IAmazonSqsMessageConsumeTopology<T> IAmazonSqsConsumeTopology.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IAmazonSqsMessageConsumeTopologyConfigurator<T>;
        }

        public void AddSpecification(IAmazonSqsConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IAmazonSqsMessageConsumeTopologyConfigurator<T> IAmazonSqsConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return base.GetMessageTopology<T>() as IAmazonSqsMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IAmazonSqsMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Bind(string topicName, Action<ITopicSubscriptionConfigurator> configure = null)
        {
            var specification = new ConsumerConsumeTopologySpecification(topicName);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new AmazonSqsMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
