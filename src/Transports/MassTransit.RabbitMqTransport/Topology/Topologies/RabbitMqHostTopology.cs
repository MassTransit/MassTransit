namespace MassTransit.RabbitMqTransport.Topology.Topologies
{
    using System;
    using Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;
    using Transports;


    public class RabbitMqHostTopology :
        HostTopology,
        IRabbitMqHostTopology
    {
        readonly IRabbitMqTopologyConfiguration _configuration;
        readonly Uri _hostAddress;
        readonly IMessageNameFormatter _messageNameFormatter;

        public RabbitMqHostTopology(IRabbitMqHostConfiguration hostConfiguration, IMessageNameFormatter messageNameFormatter, Uri hostAddress,
            IRabbitMqTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _messageNameFormatter = messageNameFormatter;
            _hostAddress = hostAddress;
            _configuration = configuration;
        }

        IRabbitMqPublishTopology IRabbitMqHostTopology.PublishTopology => _configuration.Publish;
        IRabbitMqSendTopology IRabbitMqHostTopology.SendTopology => _configuration.Send;

        IRabbitMqMessagePublishTopology<T> IRabbitMqHostTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IRabbitMqMessageSendTopology<T> IRabbitMqHostTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }

        public Uri GetDestinationAddress(string exchangeName, Action<IExchangeConfigurator> configure = null)
        {
            var address = new RabbitMqEndpointAddress(_hostAddress, new Uri($"exchange:{exchangeName}"));

            var sendSettings = new RabbitMqSendSettings(address);

            configure?.Invoke(sendSettings);

            return sendSettings.GetSendAddress(_hostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<IExchangeConfigurator> configure = null)
        {
            var exchangeName = _messageNameFormatter.GetMessageName(messageType).ToString();
            var isTemporary = TypeMetadataCache.IsTemporaryMessageType(messageType);
            var address = new RabbitMqEndpointAddress(_hostAddress, new Uri($"exchange:{exchangeName}?temporary={isTemporary}"));

            var settings = new RabbitMqSendSettings(address);

            configure?.Invoke(settings);

            return settings.GetSendAddress(_hostAddress);
        }
    }
}
