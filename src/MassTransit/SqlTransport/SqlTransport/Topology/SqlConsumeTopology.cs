#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class SqlConsumeTopology :
        ConsumeTopology,
        ISqlConsumeTopologyConfigurator
    {
        readonly ISqlPublishTopology _publishTopology;
        readonly List<ISqlConsumeTopologySpecification> _specifications;

        public SqlConsumeTopology(ISqlPublishTopology publishTopology)
            : base(255)
        {
            _publishTopology = publishTopology;

            _specifications = new List<ISqlConsumeTopologySpecification>();
        }

        ISqlMessageConsumeTopology<T> ISqlConsumeTopology.GetMessageTopology<T>()
        {
            return (ISqlMessageConsumeTopology<T>)base.GetMessageTopology<T>();
        }

        public void AddSpecification(ISqlConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        ISqlMessageConsumeTopologyConfigurator<T> ISqlConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return (ISqlMessageConsumeTopologyConfigurator<T>)base.GetMessageTopology<T>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IDbMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Subscribe(string topicName, Action<ISqlTopicSubscriptionConfigurator>? configure = null)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topicName));

            var specification = new QueueSubscriptionConsumeTopologySpecification(topicName);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>()
        {
            var messageTopology = new SqlMessageConsumeTopology<T>(_publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
