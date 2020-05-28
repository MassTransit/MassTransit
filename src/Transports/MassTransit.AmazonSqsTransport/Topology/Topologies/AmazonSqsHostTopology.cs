namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using System;
    using Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;
    using Transports;


    public class AmazonSqsHostTopology :
        HostTopology,
        IAmazonSqsHostTopology
    {
        readonly IAmazonSqsTopologyConfiguration _configuration;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly IMessageNameFormatter _messageNameFormatter;

        public AmazonSqsHostTopology(IAmazonSqsHostConfiguration hostConfiguration, IMessageNameFormatter messageNameFormatter,
            IAmazonSqsTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _messageNameFormatter = messageNameFormatter;
            _configuration = configuration;
        }

        IAmazonSqsPublishTopology IAmazonSqsHostTopology.PublishTopology => _configuration.Publish;
        IAmazonSqsSendTopology IAmazonSqsHostTopology.SendTopology => _configuration.Send;

        IAmazonSqsMessagePublishTopology<T> IAmazonSqsHostTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IAmazonSqsMessageSendTopology<T> IAmazonSqsHostTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }

        public SendSettings GetSendSettings(Uri address)
        {
            var endpointAddress = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, address);

            return _configuration.Send.GetSendSettings(endpointAddress);
        }

        public Uri GetDestinationAddress(string topicName, Action<ITopicConfigurator> configure = null)
        {
            var address = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, new Uri($"topic:{topicName}"));

            var publishSettings = new TopicPublishSettings(address);

            configure?.Invoke(publishSettings);

            return publishSettings.GetSendAddress(_hostConfiguration.HostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<ITopicConfigurator> configure = null)
        {
            var topicName = _messageNameFormatter.GetMessageName(messageType).ToString();
            var isTemporary = TypeMetadataCache.IsTemporaryMessageType(messageType);
            var address = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, new Uri($"topic:{topicName}?temporary={isTemporary}"));

            var publishSettings = new TopicPublishSettings(address);

            configure?.Invoke(publishSettings);

            return publishSettings.GetSendAddress(_hostConfiguration.HostAddress);
        }

        public override bool TryGetPublishAddress<T>(out Uri publishAddress)
        {
            if (base.TryGetPublishAddress<T>(out publishAddress))
            {
                publishAddress = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, publishAddress,
                    AmazonSqsEndpointAddress.AddressType.Topic).TopicAddress;
                return true;
            }

            return false;
        }

        public override bool TryGetPublishAddress(Type messageType, out Uri publishAddress)
        {
            if (base.TryGetPublishAddress(messageType, out publishAddress))
            {
                publishAddress = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, publishAddress,
                    AmazonSqsEndpointAddress.AddressType.Topic).TopicAddress;
                return true;
            }

            return false;
        }

    }
}
