#nullable enable
namespace MassTransit.InMemoryTransport.Configuration
{
    using System;
    using MassTransit.Configuration;


    public class InMemoryBusFactoryConfigurator :
        BusFactoryConfigurator,
        IInMemoryBusFactoryConfigurator,
        IBusFactory
    {
        readonly IInMemoryBusConfiguration _busConfiguration;
        readonly IInMemoryHostConfiguration _hostConfiguration;

        public InMemoryBusFactoryConfigurator(IInMemoryBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostConfiguration = busConfiguration.HostConfiguration;

            busConfiguration.BusEndpointConfiguration.Consume.Configurator.AutoStart = true;
        }

        public IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
        {
            var queueName = _busConfiguration.Topology.Consume.CreateTemporaryQueueName("bus");

            return _hostConfiguration.CreateReceiveEndpointConfiguration(queueName, _busConfiguration.BusEndpointConfiguration, configure);
        }

        public override bool AutoStart
        {
            set { }
        }

        public void Publish<T>(Action<IInMemoryMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IInMemoryMessagePublishTopologyConfigurator<T> configurator = _busConfiguration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Host(Action<IInMemoryHostConfigurator> configure)
        {
            configure?.Invoke(_hostConfiguration.Configurator);
        }

        public void Host(Uri baseAddress, Action<IInMemoryHostConfigurator> configure)
        {
            _hostConfiguration.BaseAddress = baseAddress;

            configure?.Invoke(_hostConfiguration.Configurator);
        }

        public new IInMemoryPublishTopologyConfigurator PublishTopology => _busConfiguration.Topology.Publish;

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IInMemoryReceiveEndpointConfigurator>? configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public int TransportConcurrencyLimit
        {
            set => ConcurrentMessageLimit = value;
        }
    }
}
