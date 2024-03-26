namespace MassTransit.SqlTransport.Topology
{
    using System;
    using MassTransit.Topology;
    using Metadata;


    public class SqlPublishTopology :
        PublishTopology,
        ISqlPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public SqlPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        ISqlMessagePublishTopology<T> ISqlPublishTopology.GetMessageTopology<T>()
        {
            return (ISqlMessagePublishTopology<T>)GetMessageTopology<T>();
        }

        ISqlMessagePublishTopologyConfigurator ISqlPublishTopologyConfigurator.GetMessageTopology(Type messageType)
        {
            return (ISqlMessagePublishTopologyConfigurator)GetMessageTopology(messageType);
        }

        public BrokerTopology GetPublishBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            ForEachMessageType<ISqlMessagePublishTopology>(x =>
            {
                x.Apply(builder);

                builder.Topic = null;
            });

            return builder.BuildBrokerTopology();
        }

        ISqlMessagePublishTopologyConfigurator<T> ISqlPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return (ISqlMessagePublishTopologyConfigurator<T>)GetMessageTopology<T>();
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>()
        {
            var messageTopology = new SqlMessagePublishTopology<T>(this, _messageTopology.GetMessageTopology<T>());

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly SqlMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly ISqlPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(ISqlPublishTopologyConfigurator publishTopology,
                SqlMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                ISqlMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}
