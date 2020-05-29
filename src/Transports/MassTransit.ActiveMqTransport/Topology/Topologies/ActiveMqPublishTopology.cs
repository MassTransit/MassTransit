namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;


    public class ActiveMqPublishTopology :
        PublishTopology,
        IActiveMqPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public ActiveMqPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IActiveMqMessagePublishTopology<T> IActiveMqPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IActiveMqMessagePublishTopology<T>;
        }

        IActiveMqMessagePublishTopologyConfigurator<T> IActiveMqPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IActiveMqMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ActiveMqMessagePublishTopology<T>(_messageTopology.GetMessageTopology<T>());

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly ActiveMqMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IActiveMqPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IActiveMqPublishTopologyConfigurator publishTopology,
                ActiveMqMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IActiveMqMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}
