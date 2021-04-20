namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using Configurators;
    using Definition;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Configurators;
    using MassTransit.Topology;
    using Topology;
    using Topology.Settings;
    using Topology.Topologies;
    using Transport;
    using Util;


    public class ActiveMqHostConfiguration :
        BaseHostConfiguration<IActiveMqReceiveEndpointConfiguration, IActiveMqReceiveEndpointConfigurator>,
        IActiveMqHostConfiguration
    {
        readonly IActiveMqBusConfiguration _busConfiguration;
        readonly Recycle<IConnectionContextSupervisor> _connectionContext;
        readonly IActiveMqHostTopology _hostTopology;
        ActiveMqHostSettings _hostSettings;

        public ActiveMqHostConfiguration(IActiveMqBusConfiguration busConfiguration, IActiveMqTopologyConfiguration topologyConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;

            _hostSettings = new ConfigurationHostSettings(new Uri("activemq://localhost"));
            _hostTopology = new ActiveMqHostTopology(this, topologyConfiguration);

            ReceiveTransportRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<ConnectionException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            _connectionContext = new Recycle<IConnectionContextSupervisor>(() => new ConnectionContextSupervisor(this, topologyConfiguration));
        }

        public override Uri HostAddress => _hostSettings.HostAddress;

        public override IRetryPolicy ReceiveTransportRetryPolicy { get; }

        public ActiveMqHostSettings Settings
        {
            get => _hostSettings;
            set => _hostSettings = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor => _connectionContext.Supervisor;

        IActiveMqHostTopology IActiveMqHostConfiguration.HostTopology => _hostTopology;

        public void ApplyEndpointDefinition(IActiveMqReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
        {
            if (definition.IsTemporary)
            {
                configurator.AutoDelete = true;
                configurator.Durable = false;
            }

            base.ApplyEndpointDefinition(configurator, definition);
        }

        public IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IActiveMqReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

            var settings = new QueueReceiveSettings(endpointConfiguration, queueName, true, false);
            return CreateReceiveEndpointConfiguration(settings, endpointConfiguration, configure);
        }

        public IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(QueueReceiveSettings settings,
            IActiveMqEndpointConfiguration endpointConfiguration, Action<IActiveMqReceiveEndpointConfigurator> configure)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (endpointConfiguration == null)
                throw new ArgumentNullException(nameof(endpointConfiguration));
            var configuration = new ActiveMqReceiveEndpointConfiguration(this, settings, endpointConfiguration);
            configure?.Invoke(configuration);
            Observers.EndpointConfigured(configuration);
            Add(configuration);
            return configuration;
        }

        public override IHostTopology HostTopology => _hostTopology;

        public override void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, configurator =>
            {
                ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public override void ReceiveEndpoint(string queueName, Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IReceiveEndpointConfigurator> configure = null)
        {
            return CreateReceiveEndpointConfiguration(queueName, configure);
        }

        public override IHost Build()
        {
            var host = new ActiveMqHost(this, _hostTopology);

            foreach (var endpointConfiguration in GetConfiguredEndpoints())
                endpointConfiguration.Build(host);

            return host;
        }
    }
}
