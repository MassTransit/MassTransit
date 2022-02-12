#nullable enable
namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class ActiveMqConsumeTopology :
        ConsumeTopology,
        IActiveMqConsumeTopologyConfigurator
    {
        readonly IActiveMqPublishTopology _publishTopology;
        readonly IList<IActiveMqConsumeTopologySpecification> _specifications;

        public ActiveMqConsumeTopology(IActiveMqPublishTopology publishTopology, IActiveMqConsumeTopology? consumeTopology = default)
        {
            _publishTopology = publishTopology;

            if (consumeTopology?.ConsumerEndpointQueueNameFormatter != null)
                ConsumerEndpointQueueNameFormatter = consumeTopology.ConsumerEndpointQueueNameFormatter;

            if (consumeTopology?.TemporaryQueueNameFormatter != null)
                TemporaryQueueNameFormatter = consumeTopology.TemporaryQueueNameFormatter;

            _specifications = new List<IActiveMqConsumeTopologySpecification>();
        }

        public IActiveMqConsumerEndpointQueueNameFormatter? ConsumerEndpointQueueNameFormatter { get; set; }
        public IActiveMqTemporaryQueueNameFormatter? TemporaryQueueNameFormatter { get; set; }

        IActiveMqMessageConsumeTopology<T> IActiveMqConsumeTopology.GetMessageTopology<T>()
        {
            return (IActiveMqMessageConsumeTopologyConfigurator<T>)base.GetMessageTopology<T>();
        }

        public void AddSpecification(IActiveMqConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IActiveMqMessageConsumeTopologyConfigurator<T> IActiveMqConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return (IActiveMqMessageConsumeTopologyConfigurator<T>)base.GetMessageTopology<T>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IActiveMqMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public void Bind(string topicName, Action<IActiveMqTopicBindingConfigurator>? configure = null)
        {
            if (string.IsNullOrEmpty(_publishTopology.VirtualTopicPrefix) || topicName.StartsWith(_publishTopology.VirtualTopicPrefix))
            {
                var specification = new ConsumerConsumeTopologySpecification(topicName, ConsumerEndpointQueueNameFormatter);

                configure?.Invoke(specification);

                _specifications.Add(specification);
            }
            else
                _specifications.Add(new InvalidActiveMqConsumeTopologySpecification("Bind", $"Only virtual topics can be bound: {topicName}"));
        }

        public override string CreateTemporaryQueueName(string tag)
        {
            var queueName = new string(base.CreateTemporaryQueueName(tag).Where(c => c != '.').ToArray());

            if (TemporaryQueueNameFormatter != null)
                queueName = TemporaryQueueNameFormatter.Format(queueName);

            return queueName;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ActiveMqMessageConsumeTopology<T>(_publishTopology.GetMessageTopology<T>(), ConsumerEndpointQueueNameFormatter);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
