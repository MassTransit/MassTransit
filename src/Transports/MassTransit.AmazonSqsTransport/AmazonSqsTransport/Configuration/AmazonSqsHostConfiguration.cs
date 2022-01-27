namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology;
    using Transports;
    using Util;


    public class AmazonSqsHostConfiguration :
        BaseHostConfiguration<IAmazonSqsReceiveEndpointConfiguration, IAmazonSqsReceiveEndpointConfigurator>,
        IAmazonSqsHostConfiguration
    {
        readonly IAmazonSqsBusConfiguration _busConfiguration;
        readonly IAmazonSqsBusTopology _busTopology;
        readonly Recycle<IConnectionContextSupervisor> _connectionContext;
        readonly IAmazonSqsTopologyConfiguration _topologyConfiguration;
        AmazonSqsHostSettings _hostSettings;

        public AmazonSqsHostConfiguration(IAmazonSqsBusConfiguration busConfiguration, IAmazonSqsTopologyConfiguration
            topologyConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _topologyConfiguration = topologyConfiguration;

            _hostSettings = new ConfigurationHostSettings();

            var messageNameFormatter = new AmazonSqsMessageNameFormatter();

            _busTopology = new AmazonSqsBusTopology(this, messageNameFormatter, topologyConfiguration);

            ReceiveTransportRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<AmazonSqsTransportException>();
                x.Handle<AmazonSqsConnectionException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            _connectionContext = new Recycle<IConnectionContextSupervisor>(() => new ConnectionContextSupervisor(this, topologyConfiguration));
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor => _connectionContext.Supervisor;

        public override Uri HostAddress => _hostSettings.HostAddress;

        public AmazonSqsHostSettings Settings
        {
            get => _hostSettings;
            set
            {
                _hostSettings = value ?? throw new ArgumentNullException(nameof(value));

                var hostAddress = new AmazonSqsHostAddress(value.HostAddress);

                if (value.ScopeTopics && hostAddress.Scope != "/")
                {
                    var formatter = new PrefixEntityNameFormatter(_topologyConfiguration.Message.EntityNameFormatter, hostAddress.Scope.Trim('/') + "_");

                    _topologyConfiguration.Message.SetEntityNameFormatter(formatter);
                }
            }
        }

        public override IBusTopology Topology => _busTopology;

        public override IRetryPolicy ReceiveTransportRetryPolicy { get; }

        public void ApplyEndpointDefinition(IAmazonSqsReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
        {
            if (definition.IsTemporary)
            {
                configurator.AutoDelete = true;
                configurator.Durable = false;
            }

            base.ApplyEndpointDefinition(configurator, definition);
        }

        public IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IAmazonSqsReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

            var settings = new QueueReceiveSettings(endpointConfiguration, queueName, true, false);

            return CreateReceiveEndpointConfiguration(settings, endpointConfiguration, configure);
        }

        public IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(QueueReceiveSettings settings,
            IAmazonSqsEndpointConfiguration endpointConfiguration, Action<IAmazonSqsReceiveEndpointConfigurator> configure)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (endpointConfiguration == null)
                throw new ArgumentNullException(nameof(endpointConfiguration));

            var configuration = new AmazonSqsReceiveEndpointConfiguration(this, settings, endpointConfiguration);

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);

            Add(configuration);

            return configuration;
        }

        IAmazonSqsBusTopology IAmazonSqsHostConfiguration.Topology => _busTopology;

        public override void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IAmazonSqsReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, configurator =>
            {
                ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public override void ReceiveEndpoint(string queueName, Action<IAmazonSqsReceiveEndpointConfigurator> configureEndpoint)
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
            var host = new AmazonSqsHost(this, _busTopology);

            foreach (var endpointConfiguration in GetConfiguredEndpoints())
                endpointConfiguration.Build(host);

            return host;
        }
    }
}
