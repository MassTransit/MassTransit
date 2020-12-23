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

            _connectionContext = new Recycle<IConnectionContextSupervisor>(() => new ConnectionContextSupervisor(this, topologyConfiguration));
        }

        public override Uri HostAddress => _hostSettings.HostAddress;

        public override IRetryPolicy ReceiveTransportRetryPolicy
        {
            get
            {
                return Retry.CreatePolicy(x =>
                {
                    x.Handle<ConnectionException>();

                    x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
                });
            }
        }

        public ActiveMqHostSettings Settings
        {
            get => _hostSettings;
            set => _hostSettings = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor => _connectionContext.Supervisor;

        IActiveMqHostTopology IActiveMqHostConfiguration.HostTopology => _hostTopology;

        public void ApplyEndpointDefinition(IActiveMqReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
        {
            configurator.ConfigureConsumeTopology = definition.ConfigureConsumeTopology;
            if (definition.IsTemporary)
            {
                configurator.AutoDelete = true;
                configurator.Durable = false;
            }

            if (definition.PrefetchCount.HasValue)
                configurator.PrefetchCount = (ushort)definition.PrefetchCount.Value;

            if (definition.ConcurrentMessageLimit.HasValue)
            {
                var concurrentMessageLimit = definition.ConcurrentMessageLimit.Value;

                // if there is a prefetchCount, and it is greater than the concurrent message limit, we need a filter
                if (!definition.PrefetchCount.HasValue || definition.PrefetchCount.Value > concurrentMessageLimit)
                {
                    configurator.UseConcurrencyLimit(concurrentMessageLimit);

                    // we should determine a good value to use based upon the concurrent message limit
                    if (definition.PrefetchCount.HasValue == false)
                    {
                        var calculatedPrefetchCount = concurrentMessageLimit * 12 / 10;

                        configurator.PrefetchCount = (ushort)calculatedPrefetchCount;
                    }
                }
            }

            definition.Configure(configurator);
        }

        public IActiveMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IActiveMqReceiveEndpointConfigurator> configure)
        {
            var settings = new QueueReceiveSettings(queueName, true, false);

            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();
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

            foreach (var endpointConfiguration in Endpoints)
                endpointConfiguration.Build(host);

            return host;
        }
    }
}
