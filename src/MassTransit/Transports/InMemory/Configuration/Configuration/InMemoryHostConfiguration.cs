namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Definition;
    using GreenPipes.Caching;
    using MassTransit.Configurators;
    using Topology.Topologies;


    public class InMemoryHostConfiguration :
        BaseHostConfiguration<IInMemoryReceiveEndpointConfiguration>,
        IInMemoryHostConfiguration,
        IInMemoryHostConfigurator
    {
        readonly IInMemoryBusConfiguration _busConfiguration;
        readonly IInMemoryTopologyConfiguration _topologyConfiguration;
        readonly ICacheConfigurator _sendTransportCacheConfigurator;
        readonly InMemoryHostProxy _proxy;
        Uri _hostAddress;

        public InMemoryHostConfiguration(IInMemoryBusConfiguration busConfiguration, Uri baseAddress, IInMemoryTopologyConfiguration topologyConfiguration)
        {
            _busConfiguration = busConfiguration;
            _topologyConfiguration = topologyConfiguration;

            _hostAddress = baseAddress ?? new Uri("loopback://localhost/");

            TransportConcurrencyLimit = Environment.ProcessorCount;

            _sendTransportCacheConfigurator = new CacheConfigurator();

            _proxy = new InMemoryHostProxy();
        }

        public IInMemoryHostConfigurator Configurator => this;

        public CacheSettings SendTransportCacheSettings => _sendTransportCacheConfigurator.Settings;
        public IInMemoryHost Proxy => _proxy;

        public override Uri HostAddress => _hostAddress;

        public Uri BaseAddress
        {
            set => _hostAddress = value ?? new Uri("loopback://localhost/");
        }

        public int TransportConcurrencyLimit { get; set; }

        public IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IInMemoryReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

            return CreateReceiveEndpointConfiguration(queueName, endpointConfiguration, configure);
        }

        public IInMemoryReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            IInMemoryEndpointConfiguration endpointConfiguration, Action<IInMemoryReceiveEndpointConfigurator> configure)
        {
            if (endpointConfiguration == null)
                throw new ArgumentNullException(nameof(endpointConfiguration));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(queueName));

            var configuration = new InMemoryReceiveEndpointConfiguration(this, queueName, endpointConfiguration);

            configuration.ConnectConsumerConfigurationObserver(_busConfiguration);
            configuration.ConnectSagaConfigurationObserver(_busConfiguration);
            configuration.ConnectHandlerConfigurationObserver(_busConfiguration);
            configuration.ConnectActivityConfigurationObserver(_busConfiguration);

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);
            Add(configuration);

            return configuration;
        }

        ICacheConfigurator IInMemoryHostConfigurator.SendTransportCache => _sendTransportCacheConfigurator;

        void IReceiveConfigurator.ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, configureEndpoint);
        }

        void IReceiveConfigurator.ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IBusHostControl Build()
        {
            var hostTopology = new InMemoryHostTopology(_topologyConfiguration);
            var host = new InMemoryHost(this, hostTopology);

            foreach (var endpointConfiguration in Endpoints)
            {
                endpointConfiguration.Build(host);
            }

            _proxy.SetComplete(host);

            return host;
        }
    }
}
