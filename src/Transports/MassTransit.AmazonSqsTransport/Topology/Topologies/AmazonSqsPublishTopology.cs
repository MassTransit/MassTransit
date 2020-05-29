namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using System;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;


    public class AmazonSqsPublishTopology :
        PublishTopology,
        IAmazonSqsPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public AmazonSqsPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IAmazonSqsMessagePublishTopology<T> IAmazonSqsPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IAmazonSqsMessagePublishTopology<T>;
        }

        IAmazonSqsMessagePublishTopologyConfigurator<T> IAmazonSqsPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IAmazonSqsMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new AmazonSqsMessagePublishTopology<T>(_messageTopology.GetMessageTopology<T>());

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly AmazonSqsMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IAmazonSqsPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IAmazonSqsPublishTopologyConfigurator publishTopology,
                AmazonSqsMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IAmazonSqsMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}
