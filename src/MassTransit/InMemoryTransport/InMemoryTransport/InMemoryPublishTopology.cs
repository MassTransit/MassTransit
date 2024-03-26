namespace MassTransit.InMemoryTransport
{
    using System;
    using Metadata;
    using Topology;


    public class InMemoryPublishTopology :
        PublishTopology,
        IInMemoryPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public InMemoryPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IInMemoryMessagePublishTopology<T> IInMemoryPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IInMemoryMessagePublishTopology<T>;
        }

        IInMemoryMessagePublishTopologyConfigurator<T> IInMemoryPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IInMemoryMessagePublishTopologyConfigurator<T>;
        }

        IInMemoryMessagePublishTopologyConfigurator IInMemoryPublishTopologyConfigurator.GetMessageTopology(Type messageType)
        {
            return GetMessageTopology(messageType) as IInMemoryMessagePublishTopologyConfigurator;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>()
        {
            var topology = new InMemoryMessagePublishTopology<T>(this, _messageTopology.GetMessageTopology<T>());

            var connector = new ImplementedMessageTypeConnector<T>(this, topology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(topology);

            return topology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly InMemoryMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IInMemoryPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IInMemoryPublishTopologyConfigurator publishTopology,
                InMemoryMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IInMemoryMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}
