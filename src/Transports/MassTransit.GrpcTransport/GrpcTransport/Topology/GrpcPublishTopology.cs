namespace MassTransit.GrpcTransport.Topology
{
    using System;
    using MassTransit.Topology;
    using Metadata;


    public class GrpcPublishTopology :
        PublishTopology,
        IGrpcPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public GrpcPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IGrpcMessagePublishTopology<T> IGrpcPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IGrpcMessagePublishTopology<T>;
        }

        IGrpcMessagePublishTopologyConfigurator<T> IGrpcPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IGrpcMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var topology = new GrpcMessagePublishTopology<T>(_messageTopology.GetMessageTopology<T>());

            var connector = new ImplementedMessageTypeConnector<T>(this, topology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(topology);

            return topology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly GrpcMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IGrpcPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IGrpcPublishTopologyConfigurator publishTopology,
                GrpcMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IGrpcMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}
