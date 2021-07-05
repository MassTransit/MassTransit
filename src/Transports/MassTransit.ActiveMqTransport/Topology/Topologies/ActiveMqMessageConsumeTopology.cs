using MassTransit.ActiveMqTransport.Configuration;

namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using GreenPipes;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Specifications;


    public class ActiveMqMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IActiveMqMessageConsumeTopologyConfigurator<TMessage>,
        IActiveMqMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        //readonly string _consumerName;
        readonly IActiveMqMessagePublishTopology<TMessage> _publishTopology;
        private readonly ActiveMqConsumeTopology _consumeTopology;
        readonly IList<IActiveMqConsumeTopologySpecification> _specifications;

        public ActiveMqMessageConsumeTopology(IActiveMqMessagePublishTopology<TMessage> publishTopology, ActiveMqConsumeTopology consumeTopology)
        {
            _publishTopology = publishTopology;
            _consumeTopology = consumeTopology;
            _specifications = new List<IActiveMqConsumeTopologySpecification>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind(Action<ITopicBindingConfigurator> configure = null)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidActiveMqConsumeTopologySpecification(TypeMetadataCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            // get the bind specification factory method
            var specificationFactoryMethod = _consumeTopology?.Topology?.BusConfiguration
                ?.BindingConsumeTopologySpecificationFactoryMethod;

            IActiveMqBindingConsumeTopologySpecification specification =null;

            // if specification factory method is null then do default behavior
            if (specificationFactoryMethod == null)
            {
                specification = new ActiveMqBindConsumeTopologySpecification(_publishTopology.Topic.EntityName);
            }
            else
            {
                // use the factory method to create the desired specification
                specification = specificationFactoryMethod(_publishTopology.Topic.EntityName);
            }

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
