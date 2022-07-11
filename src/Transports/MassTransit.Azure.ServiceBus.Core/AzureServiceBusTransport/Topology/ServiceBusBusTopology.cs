namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using Configuration;
    using Transports;


    public class ServiceBusBusTopology :
        BusTopology,
        IServiceBusBusTopology
    {
        readonly IServiceBusTopologyConfiguration _configuration;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusBusTopology(IServiceBusHostConfiguration hostConfiguration, IServiceBusTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        IServiceBusPublishTopology IServiceBusBusTopology.PublishTopology => _configuration.Publish;
        IServiceBusSendTopology IServiceBusBusTopology.SendTopology => _configuration.Send;

        public Uri GetDestinationAddress(string queueName, Action<IServiceBusQueueConfigurator> configure = null)
        {
            var configurator = new ServiceBusQueueConfigurator(queueName);

            configure?.Invoke(configurator);

            return configurator.GetQueueAddress(_hostConfiguration.HostAddress);
        }

        IServiceBusMessagePublishTopology<T> IServiceBusBusTopology.Publish<T>()
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }

        IServiceBusMessageSendTopology<T> IServiceBusBusTopology.Send<T>()
        {
            return _configuration.Send.GetMessageTopology<T>();
        }
    }
}
