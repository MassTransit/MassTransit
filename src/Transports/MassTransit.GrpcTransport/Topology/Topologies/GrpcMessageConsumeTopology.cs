namespace MassTransit.GrpcTransport.Topology.Topologies
{
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using Contracts;
    using GreenPipes;
    using GrpcTransport.Builders;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Specifications;


    public class GrpcMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IGrpcMessageConsumeTopologyConfigurator<TMessage>,
        IGrpcMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IGrpcMessagePublishTopology<TMessage> _publishTopology;
        readonly IList<IGrpcConsumeTopologySpecification> _specifications;

        public GrpcMessageConsumeTopology(IMessageTopology<TMessage> messageTopology, IGrpcMessagePublishTopology<TMessage> publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
            _specifications = new List<IGrpcConsumeTopologySpecification>();
        }

        public void Apply(IGrpcConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind(ExchangeType? exchangeType = default, string routingKey = default)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidGrpcConsumeTopologySpecification(TypeMetadataCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var specification = new ExchangeBindingConsumeTopologySpecification(_messageTopology.EntityName, exchangeType ?? _publishTopology.ExchangeType,
                routingKey);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
