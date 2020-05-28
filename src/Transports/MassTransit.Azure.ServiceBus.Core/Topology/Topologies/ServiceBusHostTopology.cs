namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using Configuration;
    using Configurators;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Transports;


    public class ServiceBusHostTopology :
        HostTopology,
        IServiceBusHostTopology
    {
        readonly IServiceBusTopologyConfiguration _configuration;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly IMessageNameFormatter _messageNameFormatter;

        public ServiceBusHostTopology(IServiceBusHostConfiguration hostConfiguration, IServiceBusTopologyConfiguration configuration,
            IMessageNameFormatter messageNameFormatter)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;

            _messageNameFormatter = messageNameFormatter ?? new ServiceBusMessageNameFormatter(false);
        }

        IServiceBusPublishTopology IServiceBusHostTopology.PublishTopology => _configuration.Publish;
        IServiceBusSendTopology IServiceBusHostTopology.SendTopology => _configuration.Send;

        public Uri GetDestinationAddress(string queueName, Action<IQueueConfigurator> configure = null)
        {
            var configurator = new QueueConfigurator(queueName);

            configure?.Invoke(configurator);

            return configurator.GetQueueAddress(_hostConfiguration.HostAddress);
        }

        public Uri GetDestinationAddress(Type messageType, Action<IQueueConfigurator> configure = null)
        {
            var queueName = _messageNameFormatter.GetMessageName(messageType).ToString();

            var configurator = new QueueConfigurator(queueName);

            if (TypeMetadataCache.IsTemporaryMessageType(messageType))
                configurator.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;

            configure?.Invoke(configurator);

            return configurator.GetQueueAddress(_hostConfiguration.HostAddress);
        }

        IServiceBusMessagePublishTopology<T> IServiceBusHostTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IServiceBusMessageSendTopology<T> IServiceBusHostTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }

        public override bool TryGetPublishAddress<T>(out Uri publishAddress)
        {
            if (base.TryGetPublishAddress<T>(out publishAddress))
            {
                publishAddress = new ServiceBusEndpointAddress(_hostConfiguration.HostAddress, publishAddress,
                    ServiceBusEndpointAddress.AddressType.Topic).TopicAddress;
                return true;
            }

            return false;
        }

        public override bool TryGetPublishAddress(Type messageType, out Uri publishAddress)
        {
            if (base.TryGetPublishAddress(messageType, out publishAddress))
            {
                publishAddress = new ServiceBusEndpointAddress(_hostConfiguration.HostAddress, publishAddress,
                    ServiceBusEndpointAddress.AddressType.Topic).TopicAddress;
                return true;
            }

            return false;
        }
    }
}
