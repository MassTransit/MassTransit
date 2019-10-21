namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Configurators;
    using Definition;
    using MassTransit.Configurators;
    using Settings;
    using Topology.Configuration.Configurators;
    using Topology.Topologies;
    using Transport;
    using Transports;


    public class ServiceBusHostConfiguration :
        BaseHostConfiguration<IServiceBusEntityEndpointConfiguration>,
        IServiceBusHostConfiguration
    {
        readonly IServiceBusBusConfiguration _busConfiguration;
        readonly ServiceBusHostProxy _proxy;
        readonly IServiceBusTopologyConfiguration _topologyConfiguration;
        ServiceBusHostSettings _hostSettings;

        public ServiceBusHostConfiguration(IServiceBusBusConfiguration busConfiguration, IServiceBusTopologyConfiguration topologyConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostSettings = new HostSettings();
            _topologyConfiguration = topologyConfiguration;

            _proxy = new ServiceBusHostProxy(this);
        }

        public override Uri HostAddress => _hostSettings.ServiceUri;

        string IServiceBusHostConfiguration.BasePath => _hostSettings.ServiceUri.AbsolutePath.Trim('/');

        public bool DeployTopologyOnly { get; set; }

        public IServiceBusHost Proxy => _proxy;

        public ServiceBusHostSettings Settings
        {
            get => _hostSettings;
            set => _hostSettings = value ?? throw new ArgumentNullException(nameof(value));
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
            Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public void ReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            var settings = new ReceiveEndpointSettings(queueName, new QueueConfigurator(queueName));

            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

            return CreateReceiveEndpointConfiguration(settings, endpointConfiguration, configure);
        }

        public IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(ReceiveEndpointSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration, Action<IServiceBusReceiveEndpointConfigurator> configure)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (endpointConfiguration == null)
                throw new ArgumentNullException(nameof(endpointConfiguration));

            var configuration = new ServiceBusReceiveEndpointConfiguration(this, settings, endpointConfiguration);

            configuration.ConnectConsumerConfigurationObserver(_busConfiguration);
            configuration.ConnectSagaConfigurationObserver(_busConfiguration);
            configuration.ConnectHandlerConfigurationObserver(_busConfiguration);
            configuration.ConnectActivityConfigurationObserver(_busConfiguration);

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);

            Add(configuration);

            return configuration;
        }

        public void SubscriptionEndpoint<T>(string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class
        {
            var settings = new SubscriptionEndpointSettings(_busConfiguration.Topology.Publish.GetMessageTopology<T>().TopicDescription, subscriptionName);

            CreateSubscriptionEndpointConfiguration(settings, configure);
        }

        public void SubscriptionEndpoint(string subscriptionName, string topicPath, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            var settings = new SubscriptionEndpointSettings(topicPath, subscriptionName);

            CreateSubscriptionEndpointConfiguration(settings, configure);
        }

        public IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(SubscriptionEndpointSettings settings,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
        {
            return CreateSubscriptionEndpointConfiguration(settings, _busConfiguration.CreateEndpointConfiguration(), configure);
        }

        public IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(SubscriptionEndpointSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (endpointConfiguration == null)
                throw new ArgumentNullException(nameof(endpointConfiguration));

            var configuration = new ServiceBusSubscriptionEndpointConfiguration(this, settings, endpointConfiguration);

            configuration.ConnectConsumerConfigurationObserver(_busConfiguration);
            configuration.ConnectSagaConfigurationObserver(_busConfiguration);
            configuration.ConnectHandlerConfigurationObserver(_busConfiguration);
            configuration.ConnectActivityConfigurationObserver(_busConfiguration);

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);

            Add(configuration);

            return configuration;
        }

        public override IBusHostControl Build()
        {
            var hostTopology = new ServiceBusHostTopology(_topologyConfiguration, _hostSettings.ServiceUri);

            var host = new ServiceBusHost(this, hostTopology);

            foreach (var endpointConfiguration in Endpoints)
            {
                endpointConfiguration.Build(host);
            }

            _proxy.SetComplete(host);

            return host;
        }
    }
}
