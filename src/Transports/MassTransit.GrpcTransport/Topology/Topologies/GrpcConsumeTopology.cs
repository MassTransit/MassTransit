namespace MassTransit.GrpcTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using GreenPipes;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;


    public class GrpcConsumeTopology :
        ConsumeTopology,
        IGrpcConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IList<IGrpcConsumeTopologySpecification> _specifications;

        public GrpcConsumeTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
            _specifications = new List<IGrpcConsumeTopologySpecification>();
        }

        IGrpcMessageConsumeTopology<T> IGrpcConsumeTopology.GetMessageTopology<T>()
        {
            IMessageConsumeTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IGrpcMessageConsumeTopology<T>;
        }

        public void AddSpecification(IGrpcConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IGrpcMessageConsumeTopologyConfigurator<T> IGrpcConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IGrpcMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(GrpcTransport.Builders.IGrpcConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IGrpcMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var topology = new GrpcMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(topology);

            return topology;
        }
    }
}
