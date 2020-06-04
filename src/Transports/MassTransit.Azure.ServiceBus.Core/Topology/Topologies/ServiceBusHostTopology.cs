namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using Configuration;
    using Configurators;
    using MassTransit.Topology.Topologies;


    public class ServiceBusHostTopology :
        HostTopology,
        IServiceBusHostTopology
    {
        readonly IServiceBusTopologyConfiguration _configuration;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusHostTopology(IServiceBusHostConfiguration hostConfiguration, IServiceBusTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        IServiceBusPublishTopology IServiceBusHostTopology.PublishTopology => _configuration.Publish;
        IServiceBusSendTopology IServiceBusHostTopology.SendTopology => _configuration.Send;

        public Uri GetDestinationAddress(string queueName, Action<IQueueConfigurator> configure = null)
        {
            var configurator = new QueueConfigurator(queueName);

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
