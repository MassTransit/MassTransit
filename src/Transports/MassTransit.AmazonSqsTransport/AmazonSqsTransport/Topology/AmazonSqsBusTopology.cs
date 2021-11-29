namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using Configuration;
    using Transports;


    public class AmazonSqsBusTopology :
        BusTopology,
        IAmazonSqsBusTopology
    {
        readonly IAmazonSqsTopologyConfiguration _configuration;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly IMessageNameFormatter _messageNameFormatter;

        public AmazonSqsBusTopology(IAmazonSqsHostConfiguration hostConfiguration, IMessageNameFormatter messageNameFormatter,
            IAmazonSqsTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _messageNameFormatter = messageNameFormatter;
            _configuration = configuration;
        }

        IAmazonSqsPublishTopology IAmazonSqsBusTopology.PublishTopology => _configuration.Publish;
        IAmazonSqsSendTopology IAmazonSqsBusTopology.SendTopology => _configuration.Send;

        IAmazonSqsMessagePublishTopology<T> IAmazonSqsBusTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IAmazonSqsMessageSendTopology<T> IAmazonSqsBusTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }

        public SendSettings GetSendSettings(Uri address)
        {
            var endpointAddress = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, address);

            return _configuration.Send.GetSendSettings(endpointAddress);
        }

        public Uri GetDestinationAddress(string topicName, Action<IAmazonSqsTopicConfigurator> configure = null)
        {
            var address = new AmazonSqsEndpointAddress(_hostConfiguration.HostAddress, new Uri($"topic:{topicName}"));

            var publishSettings = new TopicPublishSettings(address);

            configure?.Invoke(publishSettings);

            return publishSettings.GetSendAddress(_hostConfiguration.HostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<IAmazonSqsTopicConfigurator> configure = null)
        {
            var topicName = _messageNameFormatter.GetMessageName(messageType).ToString();
            var isTemporary = MessageTypeCache.IsTemporaryMessageType(messageType);
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
