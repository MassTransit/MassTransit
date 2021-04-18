namespace MassTransit.GrpcTransport.Topology.Topologies
{
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using GreenPipes;
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
        readonly IList<IGrpcConsumeTopologySpecification> _specifications;

        public GrpcMessageConsumeTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;
            _specifications = new List<IGrpcConsumeTopologySpecification>();
        }

        public void Apply(GrpcTransport.Builders.IGrpcConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind()
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidGrpcConsumeTopologySpecification(TypeMetadataCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var specification = new ExchangeBindingConsumeTopologySpecification(_messageTopology.EntityName);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
