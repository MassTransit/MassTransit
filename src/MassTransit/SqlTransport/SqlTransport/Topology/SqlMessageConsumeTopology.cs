#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class SqlMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        ISqlMessageConsumeTopologyConfigurator<TMessage>,
        IDbMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly ISqlMessagePublishTopology<TMessage> _publishTopology;
        readonly List<ISqlConsumeTopologySpecification> _specifications;

        public SqlMessageConsumeTopology(ISqlMessagePublishTopology<TMessage> publishTopology)
        {
            _publishTopology = publishTopology;

            _specifications = new List<ISqlConsumeTopologySpecification>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Subscribe(Action<ISqlTopicSubscriptionConfigurator>? configure = null)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidSqlConsumeTopologySpecification(TypeCache<TMessage>.ShortName, "Is not a consumable message type"));
                return;
            }

            var specification = new QueueSubscriptionConsumeTopologySpecification(_publishTopology.Topic);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
