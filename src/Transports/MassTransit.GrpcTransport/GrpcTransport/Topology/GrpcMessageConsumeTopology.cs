namespace MassTransit.GrpcTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using MassTransit.Configuration;
    using Transports.Fabric;


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

        public void Apply(IMessageFabricConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind(ExchangeType? exchangeType = default, string routingKey = default)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidGrpcConsumeTopologySpecification(TypeCache<TMessage>.ShortName, "Is not a bindable message type"));
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
