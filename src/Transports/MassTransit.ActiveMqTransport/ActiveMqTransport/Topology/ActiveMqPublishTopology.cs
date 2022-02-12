namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using MassTransit.Topology;
    using Metadata;


    public class ActiveMqPublishTopology :
        PublishTopology,
        IActiveMqPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public ActiveMqPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;

            VirtualTopicPrefix = "VirtualTopic.";
        }

        IActiveMqMessagePublishTopology<T> IActiveMqPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IActiveMqMessagePublishTopology<T>;
        }

        public string VirtualTopicPrefix { get; set; }

        IActiveMqMessagePublishTopologyConfigurator<T> IActiveMqPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IActiveMqMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ActiveMqMessagePublishTopology<T>(this, _messageTopology.GetMessageTopology<T>());

            var connector = new ImplementedMessageTypeConnector<T>(this);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly IActiveMqPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IActiveMqPublishTopologyConfigurator publishTopology)
            {
                _publishTopology = publishTopology;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                _publishTopology.GetMessageTopology<T>();
            }
        }
    }
}
