namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using System.Text;
    using Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;
    using Transports;


    public class ActiveMqHostTopology :
        HostTopology,
        IActiveMqHostTopology
    {
        readonly Uri _hostAddress;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly IActiveMqTopologyConfiguration _configuration;

        public ActiveMqHostTopology(IMessageNameFormatter messageNameFormatter, Uri hostAddress, IActiveMqTopologyConfiguration configuration)
            : base(configuration)
        {
            _messageNameFormatter = messageNameFormatter;
            _hostAddress = hostAddress;
            _configuration = configuration;
        }

        IActiveMqPublishTopology IActiveMqHostTopology.PublishTopology => _configuration.Publish;
        IActiveMqSendTopology IActiveMqHostTopology.SendTopology => _configuration.Send;

        IActiveMqMessagePublishTopology<T> IActiveMqHostTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IActiveMqMessageSendTopology<T> IActiveMqHostTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }

        public SendSettings GetSendSettings(Uri address)
        {
            var endpointAddress = new ActiveMqEndpointAddress(_hostAddress, address);

            return _configuration.Send.GetSendSettings(endpointAddress);
        }

        public Uri GetDestinationAddress(string topicName, Action<ITopicConfigurator> configure = null)
        {
            var address = new ActiveMqEndpointAddress(_hostAddress, new Uri($"topic:{topicName}"));

            var sendSettings = new TopicSendSettings(address);

            configure?.Invoke(sendSettings);

            return sendSettings.GetSendAddress(_hostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<ITopicConfigurator> configure = null)
        {
            var queueName = _messageNameFormatter.GetMessageName(messageType).ToString();
            var isTemporary = TypeMetadataCache.IsTemporaryMessageType(messageType);

            var address = new ActiveMqEndpointAddress(_hostAddress, new Uri($"topic:{queueName}?temporary={isTemporary}"));

            var settings = new TopicSendSettings(address);

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
