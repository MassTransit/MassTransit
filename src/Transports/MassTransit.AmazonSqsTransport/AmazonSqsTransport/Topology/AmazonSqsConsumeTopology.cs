namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


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
            return (IAmazonSqsMessageConsumeTopologyConfigurator<T>)base.GetMessageTopology<T>();
        }

        public void AddSpecification(IAmazonSqsConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IAmazonSqsMessageConsumeTopologyConfigurator<T> IAmazonSqsConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return (IAmazonSqsMessageConsumeTopologyConfigurator<T>)base.GetMessageTopology<T>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IAmazonSqsMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Bind(string topicName, Action<IAmazonSqsTopicSubscriptionConfigurator> configure = null)
        {
            var specification = new ConsumerConsumeTopologySpecification(_publishTopology, topicName);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new AmazonSqsMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), _publishTopology);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
