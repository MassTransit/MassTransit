namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using Configurators;
    using Definition;
    using MassTransit.Configurators;
    using Topology;
    using Topology.Settings;
    using Topology.Topologies;
    using Transport;
    using Transports;


    public class RabbitMqHostConfiguration :
        BaseHostConfiguration<IRabbitMqReceiveEndpointConfiguration>,
        IRabbitMqHostConfiguration
    {
        readonly IRabbitMqBusConfiguration _busConfiguration;
        readonly RabbitMqHostProxy _proxy;
        readonly IRabbitMqTopologyConfiguration _topologyConfiguration;
        RabbitMqHostSettings _hostSettings;

        public RabbitMqHostConfiguration(IRabbitMqBusConfiguration busConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration)
        {
            _busConfiguration = busConfiguration;
            _topologyConfiguration = topologyConfiguration;
            _hostSettings = new ConfigurationHostSettings()
            {
                Host = "localhost",
                Username = "guest",
                Password = "guest"
            };

            _proxy = new RabbitMqHostProxy(this);
        }

        public string Description => _hostSettings.ToDescription();

        public override Uri HostAddress => _hostSettings.HostAddress;

        public IRabbitMqHost Proxy => _proxy;

        public bool PublisherConfirmation => _hostSettings.PublisherConfirmation;
        public bool DeployTopologyOnly { get; set; }

        public RabbitMqHostSettings Settings
        {
            get => _hostSettings;
            set => _hostSettings = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            var settings = new RabbitMqReceiveSettings(queueName, _busConfiguration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, true, false);
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

            return CreateReceiveEndpointConfiguration(settings, endpointConfiguration, configure);
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(RabbitMqReceiveSettings settings,
            IRabbitMqEndpointConfiguration endpointConfiguration, Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (endpointConfiguration == null)
                throw new ArgumentNullException(nameof(endpointConfiguration));

            var configuration = new RabbitMqReceiveEndpointConfiguration(this, settings, endpointConfiguration);

            configuration.ConnectConsumerConfigurationObserver(_busConfiguration);
            configuration.ConnectSagaConfigurationObserver(_busConfiguration);
            configuration.ConnectHandlerConfigurationObserver(_busConfiguration);
            configuration.ConnectActivityConfigurationObserver(_busConfiguration);

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);

            Add(configuration);

            return configuration;
        }

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
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public void ReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IBusHostControl Build()
        {
            var exchangeTypeSelector = new FanoutExchangeTypeSelector();
            var messageNameFormatter = new RabbitMqMessageNameFormatter();

            var hostTopology = new RabbitMqHostTopology(exchangeTypeSelector, messageNameFormatter, _hostSettings.HostAddress, _topologyConfiguration);

            var host = new RabbitMqHost(this, hostTopology);

            foreach (var endpointConfiguration in Endpoints)
            {
                endpointConfiguration.Build(host);
            }

            _proxy.SetComplete(host);

            return host;
        }
    }
}
