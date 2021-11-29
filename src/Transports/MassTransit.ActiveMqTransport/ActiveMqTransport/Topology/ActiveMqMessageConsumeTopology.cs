namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class ActiveMqMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IActiveMqMessageConsumeTopologyConfigurator<TMessage>,
        IActiveMqMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IActiveMqConsumerEndpointQueueNameFormatter _consumerEndpointQueueNameFormatter;
        readonly IActiveMqMessagePublishTopology<TMessage> _publishTopology;
        readonly IList<IActiveMqConsumeTopologySpecification> _specifications;

        public ActiveMqMessageConsumeTopology(IActiveMqMessagePublishTopology<TMessage> publishTopology,
            IActiveMqConsumerEndpointQueueNameFormatter consumerEndpointQueueNameFormatter)
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

        public void Bind(Action<IActiveMqTopicBindingConfigurator> configure = null)
        {
            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidActiveMqConsumeTopologySpecification(TypeCache<TMessage>.ShortName, "Is not a bindable message type"));
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
