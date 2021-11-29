namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using MassTransit.Topology;
    using Metadata;


    public class RabbitMqPublishTopology :
        PublishTopology,
        IRabbitMqPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public RabbitMqPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
            ExchangeTypeSelector = new FanoutExchangeTypeSelector();
        }

        public IExchangeTypeSelector ExchangeTypeSelector { get; }

        public PublishBrokerTopologyOptions BrokerTopologyOptions { get; set; }

        IRabbitMqMessagePublishTopology<T> IRabbitMqPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IRabbitMqMessagePublishTopologyConfigurator<T>;
        }

        IRabbitMqMessagePublishTopologyConfigurator<T> IRabbitMqPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IRabbitMqMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var exchangeTypeSelector = new MessageExchangeTypeSelector<T>(ExchangeTypeSelector);

            var messageTopology = new RabbitMqMessagePublishTopology<T>(this, _messageTopology.GetMessageTopology<T>(), exchangeTypeSelector);

            var connector = new ImplementedMessageTypeConnector<T>(this, messageTopology);

            ImplementedMessageTypeCache<T>.EnumerateImplementedTypes(connector);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }


        class ImplementedMessageTypeConnector<TMessage> :
            IImplementedMessageType
            where TMessage : class
        {
            readonly RabbitMqMessagePublishTopology<TMessage> _messagePublishTopologyConfigurator;
            readonly IRabbitMqPublishTopologyConfigurator _publishTopology;

            public ImplementedMessageTypeConnector(IRabbitMqPublishTopologyConfigurator publishTopology,
                RabbitMqMessagePublishTopology<TMessage> messagePublishTopologyConfigurator)
            {
                _publishTopology = publishTopology;
                _messagePublishTopologyConfigurator = messagePublishTopologyConfigurator;
            }

            public void ImplementsMessageType<T>(bool direct)
                where T : class
            {
                IRabbitMqMessagePublishTopologyConfigurator<T> messageTopology = _publishTopology.GetMessageTopology<T>();

                _messagePublishTopologyConfigurator.AddImplementedMessageConfigurator(messageTopology, direct);
            }
        }
    }
}
