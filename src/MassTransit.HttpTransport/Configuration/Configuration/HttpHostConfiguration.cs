namespace MassTransit.HttpTransport.Configuration
{
    using System;
    using Configurators;
    using Definition;
    using Hosting;
    using Topology;
    using Transport;
    using Transports;


    public class HttpHostConfiguration :
        BaseHostConfiguration<IHttpReceiveEndpointConfiguration>,
        IHttpHostConfiguration
    {
        readonly IHttpBusConfiguration _busConfiguration;
        readonly IHttpTopologyConfiguration _topologyConfiguration;
        HttpHostSettings _hostSettings;

        public HttpHostConfiguration(IHttpBusConfiguration busConfiguration, IHttpTopologyConfiguration topologyConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostSettings = new ConfigurationHostSettings();
            _topologyConfiguration = topologyConfiguration;
        }

        public override Uri HostAddress => _hostSettings.GetInputAddress();

        public HttpHostSettings Settings
        {
            get => _hostSettings;
            set => _hostSettings = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IHttpReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string pathMatch, Action<IHttpReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

            return CreateReceiveEndpointConfiguration(pathMatch, endpointConfiguration, configure);
        }

        public IHttpReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string pathMatch, IHttpEndpointConfiguration endpointConfiguration,
            Action<IHttpReceiveEndpointConfigurator> configure)
        {
            if (pathMatch == null)
                throw new ArgumentNullException(nameof(pathMatch));

            var configuration = new HttpReceiveEndpointConfiguration(this, pathMatch, endpointConfiguration);

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
            Action<IHttpReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public void ReceiveEndpoint(string queueName, Action<IHttpReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IBusHostControl Build()
        {
            var hostTopology = new HttpHostTopology(_topologyConfiguration);

            var host = new HttpHost(this, hostTopology);

            foreach (var endpointConfiguration in Endpoints)
            {
                endpointConfiguration.Build(host);
            }

            return host;
        }
    }
}
