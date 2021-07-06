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
        readonly IActiveMqMessagePublishTopology<TMessage> _publishTopology;
        readonly IList<IActiveMqConsumeTopologySpecification> _specifications;
        readonly IActiveMqConsumerEndpointQueueNameFormatter _consumerEndpointQueueNameFormatter;

        public ActiveMqMessageConsumeTopology(IActiveMqMessagePublishTopology<TMessage> publishTopology,IActiveMqConsumerEndpointQueueNameFormatter consumerEndpointQueueNameFormatter)
        {
            _publishTopology = publishTopology;
            
            _specifications = new List<IActiveMqConsumeTopologySpecification>();

            _consumerEndpointQueueNameFormatter = consumerEndpointQueueNameFormatter;
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

            var specification = new ConsumerConsumeTopologySpecification(_publishTopology.Topic, _consumerEndpointQueueNameFormatter);

            configure?.Invoke(specification);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
