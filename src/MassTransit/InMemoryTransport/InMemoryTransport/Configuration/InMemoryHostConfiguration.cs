namespace MassTransit.InMemoryTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Transports;
    using Util;


    public class InMemoryHostConfiguration :
        BaseHostConfiguration<IInMemoryReceiveEndpointConfiguration, IInMemoryReceiveEndpointConfigurator>,
        IInMemoryHostConfiguration,
        IInMemoryHostConfigurator
    {
        readonly IInMemoryBusConfiguration _busConfiguration;
        readonly InMemoryBusTopology _topology;
        readonly Recycle<IInMemoryTransportProvider> _transportProvider;
        Uri _hostAddress;

        public InMemoryHostConfiguration(IInMemoryBusConfiguration busConfiguration, Uri baseAddress, IInMemoryTopologyConfiguration topologyConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;

            _hostAddress = baseAddress ?? new Uri("loopback://localhost/");
            _topology = new InMemoryBusTopology(this, topologyConfiguration);

            ReceiveTransportRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<ConnectionException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            _transportProvider = new Recycle<IInMemoryTransportProvider>(() => new InMemoryTransportProvider(this, topologyConfiguration));
        }

        public override Uri HostAddress => _hostAddress;
        public override IBusTopology Topology => _topology;

        public override IRetryPolicy ReceiveTransportRetryPolicy { get; }

        public Uri BaseAddress
        {
            set => _hostAddress = value ?? new Uri("loopback://localhost/");
        }

        IInMemoryHostConfigurator IInMemoryHostConfiguration.Configurator => this;
        IInMemoryTransportProvider IInMemoryHostConfiguration.TransportProvider => _transportProvider.Supervisor;
        IInMemoryBusTopology IInMemoryHostConfiguration.Topology => _topology;

        public void ApplyEndpointDefinition(IInMemoryReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
        {
            base.ApplyEndpointDefinition(configurator, definition);
        }

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

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);
            Add(configuration);

            return configuration;
        }

        public override void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, configurator =>
            {
                ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public override void ReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName, Action<IReceiveEndpointConfigurator> configure)
        {
            return CreateReceiveEndpointConfiguration(queueName, configure);
        }

        public override IHost Build()
        {
            var host = new InMemoryHost(this, _topology);

            foreach (var endpointConfiguration in GetConfiguredEndpoints())
                endpointConfiguration.Build(host);

            return host;
        }
    }
}
