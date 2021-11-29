namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using Configuration;
    using MassTransit.Topology;
    using Transports;


    public class RabbitMqBusTopology :
        BusTopology,
        IRabbitMqBusTopology
    {
        readonly IRabbitMqTopologyConfiguration _configuration;
        readonly Uri _hostAddress;
        readonly IMessageNameFormatter _messageNameFormatter;

        public RabbitMqBusTopology(IRabbitMqHostConfiguration hostConfiguration, IMessageNameFormatter messageNameFormatter, Uri hostAddress,
            IRabbitMqTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _messageNameFormatter = messageNameFormatter;
            _hostAddress = hostAddress;
            _configuration = configuration;
        }

        IRabbitMqPublishTopology IRabbitMqBusTopology.PublishTopology => _configuration.Publish;
        IRabbitMqSendTopology IRabbitMqBusTopology.SendTopology => _configuration.Send;

        IRabbitMqMessagePublishTopology<T> IRabbitMqBusTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IRabbitMqMessageSendTopology<T> IRabbitMqBusTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }

        public Uri GetDestinationAddress(string exchangeName, Action<IRabbitMqExchangeConfigurator> configure = null)
        {
            var address = new RabbitMqEndpointAddress(_hostAddress, new Uri($"exchange:{exchangeName}"));

            var sendSettings = new RabbitMqSendSettings(address);

            configure?.Invoke(sendSettings);

            return sendSettings.GetSendAddress(_hostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<IRabbitMqExchangeConfigurator> configure = null)
        {
            var exchangeName = _messageNameFormatter.GetMessageName(messageType).ToString();
            var isTemporary = MessageTypeCache.IsTemporaryMessageType(messageType);
            var address = new RabbitMqEndpointAddress(_hostAddress, new Uri($"exchange:{exchangeName}?temporary={isTemporary}"));

            var settings = new RabbitMqSendSettings(address);

            configure?.Invoke(settings);

            return settings.GetSendAddress(_hostAddress);
        }
    }
}
