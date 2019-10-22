namespace MassTransit.RabbitMqTransport.Topology.Topologies
{
    using System;
    using System.Text;
    using Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;
    using Transports;


    public class RabbitMqHostTopology :
        HostTopology,
        IRabbitMqHostTopology
    {
        readonly IExchangeTypeSelector _exchangeTypeSelector;
        readonly Uri _hostAddress;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly IRabbitMqTopologyConfiguration _configuration;

        public RabbitMqHostTopology(IExchangeTypeSelector exchangeTypeSelector, IMessageNameFormatter messageNameFormatter,
            Uri hostAddress, IRabbitMqTopologyConfiguration configuration)
            : base(configuration)
        {
            _exchangeTypeSelector = exchangeTypeSelector;
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

        public SendSettings GetSendSettings(Uri address)
        {
            return _configuration.Send.GetSendSettings(address);
        }

        public Uri GetDestinationAddress(string exchangeName, Action<IExchangeConfigurator> configure = null)
        {
            var sendSettings = new RabbitMqSendSettings(exchangeName, _exchangeTypeSelector.DefaultExchangeType, true, false);

            configure?.Invoke(sendSettings);

            return sendSettings.GetSendAddress(_hostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<IExchangeConfigurator> configure = null)
        {
            var isTemporary = TypeMetadataCache.IsTemporaryMessageType(messageType);

            var durable = !isTemporary;
            var autoDelete = isTemporary;

            var name = _messageNameFormatter.GetMessageName(messageType).ToString();

            var settings = new RabbitMqSendSettings(name, _exchangeTypeSelector.DefaultExchangeType, durable, autoDelete);

            configure?.Invoke(settings);

            return settings.GetSendAddress(_hostAddress);
        }

        public override string CreateTemporaryQueueName(string prefix)
        {
            var sb = new StringBuilder(prefix);

            var host = HostMetadataCache.Host;

            foreach (var c in host.MachineName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            sb.Append('-');
            foreach (var c in host.ProcessName)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c == '.' || c == '_' || c == '-' || c == ':')
                    sb.Append(c);
            sb.Append('-');
            sb.Append(NewId.Next().ToString(Formatter));

            return sb.ToString();
        }
    }
}
