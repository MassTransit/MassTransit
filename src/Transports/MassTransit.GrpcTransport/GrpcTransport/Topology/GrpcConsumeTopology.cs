namespace MassTransit.GrpcTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using MassTransit.Configuration;
    using Transports.Fabric;


    public class GrpcConsumeTopology :
        ConsumeTopology,
        IGrpcConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IGrpcPublishTopologyConfigurator _publishTopology;
        readonly IList<IGrpcConsumeTopologySpecification> _specifications;

        public GrpcConsumeTopology(IMessageTopology messageTopology, IGrpcPublishTopologyConfigurator publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;
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

        public void Apply(IMessageFabricConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IGrpcMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        public void Bind(string exchangeName, ExchangeType exchangeType = ExchangeType.FanOut, string routingKey = default)
        {
            var specification = new ExchangeBindingConsumeTopologySpecification(exchangeName, exchangeType, routingKey);

            _specifications.Add(specification);
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var topology = new GrpcMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>(), _publishTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(topology);

            return topology;
        }
    }
}
