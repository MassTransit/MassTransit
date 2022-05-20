namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology;
    using Transports;
    using Util;


    public class ActiveMqHostConfiguration :
        BaseHostConfiguration<IActiveMqReceiveEndpointConfiguration, IActiveMqReceiveEndpointConfigurator>,
        IActiveMqHostConfiguration
    {
        readonly IActiveMqBusConfiguration _busConfiguration;
        readonly Recycle<IConnectionContextSupervisor> _connectionContext;
        readonly IActiveMqBusTopology _topology;
        ActiveMqHostSettings _hostSettings;

        public ActiveMqHostConfiguration(IActiveMqBusConfiguration busConfiguration, IActiveMqTopologyConfiguration topologyConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;

            _hostSettings = new ConfigurationHostSettings(new Uri("activemq://localhost"));
            _topology = new ActiveMqBusTopology(this, topologyConfiguration);

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

        public bool IsArtemis { get; set; }

        public IConnectionContextSupervisor ConnectionContextSupervisor => _connectionContext.Supervisor;

        IActiveMqBusTopology IActiveMqHostConfiguration.Topology => _topology;

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

            var settings = new ActiveMqQueueReceiveSettings(endpointConfiguration, queueName, true, false);
            return CreateReceiveEndpointConfiguration(settings, endpointConfiguration, configure);
        }

        public IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(ActiveMqQueueReceiveSettings settings,
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

        public override IBusTopology Topology => _topology;

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
            var host = new ActiveMqHost(this, _topology);

            foreach (var endpointConfiguration in GetConfiguredEndpoints())
                endpointConfiguration.Build(host);

            return host;
        }
    }
}
