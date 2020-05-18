namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using Configurators;
    using Definition;
    using MassTransit.Configurators;
    using MassTransit.Topology.EntityNameFormatters;
    using Microsoft.Azure.ServiceBus.Primitives;
    using Settings;
    using Topology.Configurators;
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
        IMessageNameFormatter _messageNameFormatter;

        public ServiceBusHostConfiguration(IServiceBusBusConfiguration busConfiguration, IServiceBusTopologyConfiguration topologyConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostSettings = new HostSettings();
            _topologyConfiguration = topologyConfiguration;

            _proxy = new ServiceBusHostProxy(this);
        }

        public override Uri HostAddress => _hostSettings.ServiceUri;

        string IServiceBusHostConfiguration.BasePath => _hostSettings.ServiceUri.AbsolutePath.Trim('/');

        public bool DeployTopologyOnly { get; set; }

        public IServiceBusHostControl Proxy => _proxy;

        public ServiceBusHostSettings Settings
        {
            get => _hostSettings;
            set
            {
                _hostSettings = value ?? throw new ArgumentNullException(nameof(value));

                if (_hostSettings.TokenProvider is ManagedIdentityTokenProvider)
                {
                    SetNamespaceSeparatorToTilde();
                }
            }
        }

        public void SetNamespaceSeparatorToTilde()
        {
            _messageNameFormatter = new ServiceBusMessageNameFormatter(true);
            _topologyConfiguration.Message.SetEntityNameFormatter(new MessageNameFormatterEntityNameFormatter(_messageNameFormatter));
        }

        public void SetNamespaceSeparatorTo(string separator)
        {
            _messageNameFormatter = new DefaultMessageNameFormatter("---", "--", separator ?? throw new ArgumentNullException(nameof(separator)), "-");
            _topologyConfiguration.Message.SetEntityNameFormatter(new MessageNameFormatterEntityNameFormatter(_messageNameFormatter));
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

            ReceiveEndpoint(queueName, configurator =>
            {
                ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public void ReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public void ApplyEndpointDefinition(IServiceBusReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
        {
            configurator.ConfigureConsumeTopology = definition.ConfigureConsumeTopology;

            if (definition.IsTemporary)
            {
                configurator.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
                configurator.RemoveSubscriptions = true;
            }

            if (definition.PrefetchCount.HasValue)
            {
                configurator.PrefetchCount = (ushort)definition.PrefetchCount.Value;
            }

            if (definition.ConcurrentMessageLimit.HasValue)
            {
                var concurrentMessageLimit = definition.ConcurrentMessageLimit.Value;

                // if there is a prefetchCount, and it is greater than the concurrent message limit, we need a filter
                if (!definition.PrefetchCount.HasValue || definition.PrefetchCount.Value > concurrentMessageLimit)
                {
                    configurator.MaxConcurrentCalls = concurrentMessageLimit;

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

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);

            Add(configuration);

            return configuration;
        }

        public override IBusHostControl Build()
        {
            var hostTopology = new ServiceBusHostTopology(_topologyConfiguration, _hostSettings.ServiceUri, _messageNameFormatter);

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
