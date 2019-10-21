namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using Configurators;
    using Definition;
    using MassTransit.Configurators;
    using Topology.Settings;
    using Topology.Topologies;
    using Transport;
    using Transports;


    public class ActiveMqHostConfiguration :
        BaseHostConfiguration<IActiveMqReceiveEndpointConfiguration>,
        IActiveMqHostConfiguration
    {
        readonly IActiveMqBusConfiguration _busConfiguration;
        readonly ActiveMqHostProxy _proxy;
        readonly IActiveMqTopologyConfiguration _topologyConfiguration;
        ActiveMqHostSettings _hostSettings;

        public ActiveMqHostConfiguration(IActiveMqBusConfiguration busConfiguration,
            IActiveMqTopologyConfiguration topologyConfiguration)
        {
            _busConfiguration = busConfiguration;
            _topologyConfiguration = topologyConfiguration;
            _hostSettings = new ConfigurationHostSettings {Host = "localhost"};

            _proxy = new ActiveMqHostProxy(this);
        }

        public string Description => _hostSettings.ToDescription();
        public override Uri HostAddress => _hostSettings.HostAddress;
        public IActiveMqHost Proxy => _proxy;
        public bool DeployTopologyOnly { get; set; }

        public ActiveMqHostSettings Settings
        {
            get => _hostSettings;
            set => _hostSettings = value ?? throw new ArgumentNullException(nameof(value));
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
            Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public void ReceiveEndpoint(string queueName, Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IBusHostControl Build()
        {
            var messageNameFormatter = new ActiveMqMessageNameFormatter();

            var hostTopology = new ActiveMqHostTopology(messageNameFormatter, _hostSettings.HostAddress, _topologyConfiguration);

            var host = new ActiveMqHost(this, hostTopology);

            foreach (var endpointConfiguration in Endpoints)
            {
                endpointConfiguration.Build(host);
            }

            _proxy.SetComplete(host);

            return host;
        }
    }
}
