namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;


    public class ServiceBusPublishTopology :
        PublishTopology,
        IServiceBusPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public ServiceBusPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
        }

        IServiceBusMessagePublishTopology<T> IServiceBusPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessagePublishTopologyConfigurator<T>;
        }

        IServiceBusMessagePublishTopologyConfigurator<T> IServiceBusPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IServiceBusMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new ServiceBusMessagePublishTopology<T>(_messageTopology.GetMessageTopology<T>());

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly ServiceBusMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IServiceBusPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IServiceBusPublishTopologyConfigurator publishTopology,
                ServiceBusMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IServiceBusMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}
