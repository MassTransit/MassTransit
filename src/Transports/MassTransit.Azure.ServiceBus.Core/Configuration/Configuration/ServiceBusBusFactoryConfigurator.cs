#nullable enable
namespace MassTransit.Configuration
{
    using System;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;
    using AzureServiceBusTransport.Topology;


    public class ServiceBusBusFactoryConfigurator :
        BusFactoryConfigurator,
        IServiceBusBusFactoryConfigurator,
        IBusFactory
    {
        readonly IServiceBusBusConfiguration _busConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly ServiceBusQueueConfigurator _queueConfigurator;
        readonly ReceiveEndpointSettings _settings;

        public ServiceBusBusFactoryConfigurator(IServiceBusBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostConfiguration = busConfiguration.HostConfiguration;

            var queueName = _busConfiguration.Topology.Consume.CreateTemporaryQueueName("bus");

            _queueConfigurator = new ServiceBusQueueConfigurator(queueName) { AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle };

            _settings = new ReceiveEndpointSettings(_busConfiguration.BusEndpointConfiguration, queueName, _queueConfigurator);
        }

        public IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
        {
            return _busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration(_settings, _busConfiguration.BusEndpointConfiguration, configure);
        }

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            set => _queueConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        public bool EnablePartitioning
        {
            set => _queueConfigurator.EnablePartitioning = value;
        }

        public long MaxSizeInMegabytes
        {
            set => _queueConfigurator.MaxSizeInMegabytes = value;
        }

        public long MaxMessageSizeInKilobytes
        {
            set => _settings.QueueConfigurator.MaxMessageSizeInKilobytes = value;
        }

        public bool RequiresDuplicateDetection
        {
            set => _queueConfigurator.RequiresDuplicateDetection = value;
        }

        public int MaxConcurrentCalls
        {
            set => ConcurrentMessageLimit = value;
        }

        public void OverrideDefaultBusEndpointQueueName(string value)
        {
            _queueConfigurator.Path = value;
        }

        public void SetNamespaceSeparatorToTilde()
        {
            _hostConfiguration.SetNamespaceSeparatorToTilde();
        }

        public void SetNamespaceSeparatorToUnderscore()
        {
            _hostConfiguration.SetNamespaceSeparatorToUnderscore();
        }

        public void SetNamespaceSeparatorTo(string separator)
        {
            _hostConfiguration.SetNamespaceSeparatorTo(separator);
        }

        public void Send<T>(Action<IServiceBusMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IServiceBusMessageSendTopologyConfigurator<T> configurator = _busConfiguration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Publish<T>(Action<IServiceBusMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class
        {
            IServiceBusMessagePublishTopologyConfigurator<T> configurator = _busConfiguration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IServiceBusSendTopologyConfigurator SendTopology => _busConfiguration.Topology.Send;
        public new IServiceBusPublishTopologyConfigurator PublishTopology => _busConfiguration.Topology.Publish;

        public void Host(ServiceBusHostSettings settings)
        {
            _busConfiguration.HostConfiguration.Settings = settings;
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IServiceBusReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void SubscriptionEndpoint<T>(string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class
        {
            _hostConfiguration.SubscriptionEndpoint<T>(subscriptionName, configure);
        }

        public void SubscriptionEndpoint(string subscriptionName, string topicPath, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
        {
            _hostConfiguration.SubscriptionEndpoint(subscriptionName, topicPath, configure);
        }

        public TimeSpan AutoDeleteOnIdle
        {
            set => _queueConfigurator.AutoDeleteOnIdle = value;
        }

        public TimeSpan DefaultMessageTimeToLive
        {
            set => _queueConfigurator.DefaultMessageTimeToLive = value;
        }

        public bool EnableBatchedOperations
        {
            set => _queueConfigurator.EnableBatchedOperations = value;
        }

        public bool EnableDeadLetteringOnMessageExpiration
        {
            set => _queueConfigurator.EnableDeadLetteringOnMessageExpiration = value;
        }

        public string ForwardDeadLetteredMessagesTo
        {
            set => _queueConfigurator.ForwardDeadLetteredMessagesTo = value;
        }

        public TimeSpan LockDuration
        {
            set => _queueConfigurator.LockDuration = value;
        }

        public int MaxDeliveryCount
        {
            set => _queueConfigurator.MaxDeliveryCount = value;
        }

        public bool RequiresSession
        {
            set => _queueConfigurator.RequiresSession = value;
        }

        public int MaxConcurrentCallsPerSession
        {
            set => _queueConfigurator.MaxConcurrentCallsPerSession = value;
        }

        public string UserMetadata
        {
            set => _queueConfigurator.UserMetadata = value;
        }

        public void SelectBasicTier()
        {
            _settings.SelectBasicTier();
        }

        public TimeSpan MessageWaitTimeout
        {
            set => _settings.SessionIdleTimeout = value;
        }

        public TimeSpan SessionIdleTimeout
        {
            set => _settings.SessionIdleTimeout = value;
        }

        public TimeSpan MaxAutoRenewDuration
        {
            set => _settings.MaxAutoRenewDuration = value;
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _queueConfigurator.RequiresDuplicateDetection = true;
            _queueConfigurator.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }
    }
}
