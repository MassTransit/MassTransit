namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using Configuration;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;


    public class ActiveMqHostTopology :
        HostTopology,
        IActiveMqHostTopology
    {
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly IActiveMqTopologyConfiguration _topologyConfiguration;

        public ActiveMqHostTopology(IActiveMqHostConfiguration hostConfiguration, IActiveMqTopologyConfiguration topologyConfiguration)
            : base(hostConfiguration, topologyConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _topologyConfiguration = topologyConfiguration;
        }

        IActiveMqPublishTopology IActiveMqHostTopology.PublishTopology => _topologyConfiguration.Publish;
        IActiveMqSendTopology IActiveMqHostTopology.SendTopology => _topologyConfiguration.Send;

        IActiveMqMessagePublishTopology<T> IActiveMqHostTopology.Publish<T>()
        {
            return _topologyConfiguration.Publish.GetMessageTopology<T>();
        }

        IActiveMqMessageSendTopology<T> IActiveMqHostTopology.Send<T>()
        {
            return _topologyConfiguration.Send.GetMessageTopology<T>();
        }

        public SendSettings GetSendSettings(Uri address)
        {
            var endpointAddress = new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, address);

            return _topologyConfiguration.Send.GetSendSettings(endpointAddress);
        }

        public Uri GetDestinationAddress(string topicName, Action<ITopicConfigurator> configure = null)
        {
            var address = new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, new Uri($"topic:{topicName}"));

            var sendSettings = new TopicSendSettings(address);

            configure?.Invoke(sendSettings);

            return sendSettings.GetSendAddress(_hostConfiguration.HostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<ITopicConfigurator> configure = null)
        {
            var isTemporary = TypeMetadataCache.IsTemporaryMessageType(messageType);

            _topologyConfiguration.Publish.TryGetPublishAddress(messageType, _hostConfiguration.HostAddress, out var address);

            var settings = new TopicSendSettings(new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, address));
            if (isTemporary)
            {
                settings.AutoDelete = true;
                settings.Durable = false;
            }

            configure?.Invoke(settings);

            return settings.GetSendAddress(_hostConfiguration.HostAddress);
        }

        public override bool TryGetPublishAddress<T>(out Uri publishAddress)
        {
            if (base.TryGetPublishAddress<T>(out publishAddress))
            {
                publishAddress = new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, publishAddress).TopicAddress;
                return true;
            }

            return false;
        }

        public override bool TryGetPublishAddress(Type messageType, out Uri publishAddress)
        {
            if (base.TryGetPublishAddress(messageType, out publishAddress))
            {
                publishAddress = new ActiveMqEndpointAddress(_hostConfiguration.HostAddress, publishAddress).TopicAddress;
                return true;
            }

            return false;
        }
    }
}
