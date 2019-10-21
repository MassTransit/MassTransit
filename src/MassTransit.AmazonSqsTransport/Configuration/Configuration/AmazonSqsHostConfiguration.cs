namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using System;
    using Configurators;
    using Definition;
    using MassTransit.Configurators;
    using Topology.Settings;
    using Topology.Topologies;
    using Transport;
    using Transports;


    public class AmazonSqsHostConfiguration :
        BaseHostConfiguration<IAmazonSqsReceiveEndpointConfiguration>,
        IAmazonSqsHostConfiguration
    {
        readonly IAmazonSqsBusConfiguration _busConfiguration;
        readonly IAmazonSqsTopologyConfiguration _topologyConfiguration;
        readonly AmazonSqsHostProxy _proxy;
        AmazonSqsHostSettings _hostSettings;

        public AmazonSqsHostConfiguration(IAmazonSqsBusConfiguration busConfiguration, IAmazonSqsTopologyConfiguration
            topologyConfiguration)
        {
            _busConfiguration = busConfiguration;
            _topologyConfiguration = topologyConfiguration;
            _hostSettings = new ConfigurationHostSettings();

            _proxy = new AmazonSqsHostProxy(this);
        }

        public override Uri HostAddress => _hostSettings.HostAddress;
        public IAmazonSqsHost Proxy => _proxy;
        public bool DeployTopologyOnly { get; set; }

        public AmazonSqsHostSettings Settings
        {
            get => _hostSettings;
            set => _hostSettings = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IAmazonSqsReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IAmazonSqsReceiveEndpointConfigurator> configure)
        {
            var settings = new QueueReceiveSettings(queueName, true, false);
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

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
            Action<IAmazonSqsReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public void ReceiveEndpoint(string queueName, Action<IAmazonSqsReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IBusHostControl Build()
        {
            var messageNameFormatter = new AmazonSqsMessageNameFormatter();

            var hostTopology = new AmazonSqsHostTopology(messageNameFormatter, _hostSettings.HostAddress, _topologyConfiguration);

            var host = new AmazonSqsHost(this, hostTopology);

            foreach (var endpointConfiguration in Endpoints)
            {
                endpointConfiguration.Build(host);
            }

            _proxy.SetComplete(host);

            return host;
        }
    }
}
