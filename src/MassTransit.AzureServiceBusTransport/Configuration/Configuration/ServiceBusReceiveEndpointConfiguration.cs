namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Settings;
    using Topology.Configuration;
    using Topology.Configuration.Configurators;
    using Transport;
    using Transports;


    public class ServiceBusReceiveEndpointConfiguration :
        ServiceBusEntityReceiveEndpointConfiguration,
        IServiceBusReceiveEndpointConfiguration,
        IServiceBusReceiveEndpointConfigurator
    {
        readonly IServiceBusEndpointConfiguration _endpointConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly ReceiveEndpointSettings _settings;

        public ServiceBusReceiveEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration, IServiceBusEndpointConfiguration endpointConfiguration,
            string queueName)
            : this(hostConfiguration, endpointConfiguration, new ReceiveEndpointSettings(queueName, new QueueConfigurator(queueName)))
        {
        }

        public ServiceBusReceiveEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration, IServiceBusEndpointConfiguration endpointConfiguration,
            ReceiveEndpointSettings settings)
            : base(hostConfiguration, endpointConfiguration, settings)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
            _settings = settings;

            _settings.QueueConfigurator.BasePath = hostConfiguration.Host.BasePath;

            HostAddress = hostConfiguration.Host.Address;
            InputAddress = settings.GetInputAddress(hostConfiguration.HostAddress, settings.Name);

            SubscribeMessageTopics = true;
        }

        IServiceBusReceiveEndpointConfigurator IServiceBusReceiveEndpointConfiguration.Configurator => this;

        IServiceBusTopologyConfiguration IServiceBusEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public IServiceBusEndpointConfiguration CreateEndpointConfiguration()
        {
            return _endpointConfiguration.CreateEndpointConfiguration();
        }

        public override Uri HostAddress { get; }

        public override Uri InputAddress { get; }

        public bool EnableExpress
        {
            set
            {
                _settings.QueueConfigurator.EnableExpress = value;

                Changed(nameof(EnableExpress));
            }
        }

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            set => _settings.QueueConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        public void EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _settings.QueueConfigurator.RequiresDuplicateDetection = true;
            _settings.QueueConfigurator.DuplicateDetectionHistoryTimeWindow = historyTimeWindow;
        }

        public bool EnablePartitioning
        {
            set => _settings.QueueConfigurator.EnablePartitioning = value;
        }

        public bool IsAnonymousAccessible
        {
            set => _settings.QueueConfigurator.IsAnonymousAccessible = value;
        }

        public int MaxSizeInMegabytes
        {
            set => _settings.QueueConfigurator.MaxSizeInMegabytes = value;
        }

        public bool RequiresDuplicateDetection
        {
            set => _settings.QueueConfigurator.RequiresDuplicateDetection = value;
        }

        public bool SupportOrdering
        {
            set => _settings.QueueConfigurator.SupportOrdering = value;
        }

        public bool RemoveSubscriptions
        {
            set => _settings.RemoveSubscriptions = value;
        }

        public void Subscribe(string topicName, string subscriptionName, Action<ISubscriptionConfigurator> callback)
        {
            _endpointConfiguration.Topology.Consume.Subscribe(topicName, subscriptionName, callback);
        }

        public void Subscribe<T>(string subscriptionName, Action<ISubscriptionConfigurator> callback)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Subscribe(subscriptionName, callback);
        }

        public override void SelectBasicTier()
        {
            base.SelectBasicTier();

            SubscribeMessageTopics = false;
        }

        public IServiceBusHost Host => _hostConfiguration.Host;

        public bool SubscribeMessageTopics { get; set; }

        public ReceiveSettings Settings => _settings;

        TimeSpan IServiceBusQueueEndpointConfigurator.MessageWaitTimeout
        {
            set => _settings.MessageWaitTimeout = value;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return _settings.QueueConfigurator.Validate()
                .Concat(base.Validate());
        }

        public override IReceiveEndpoint Build()
        {
            var builder = new ServiceBusReceiveEndpointBuilder(this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            NamespacePipeConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointContext.BrokerTopology,
                _settings.RemoveSubscriptions, _hostConfiguration.Host.Stopping));

            return CreateReceiveEndpoint(receiveEndpointContext);
        }

        protected override IErrorTransport CreateErrorTransport(IServiceBusHostControl host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings.QueueConfigurator);

            return new BrokeredMessageErrorTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport(IServiceBusHostControl host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.QueueConfigurator);

            return new BrokeredMessageDeadLetterTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings,
            IPipe<NamespaceContext> namespacePipe)
        {
            return new QueueSendEndpointContextFactory(host.MessagingFactoryContextSupervisor, host.NamespaceContextSupervisor,
                Pipe.Empty<MessagingFactoryContext>(), namespacePipe,
                settings);
        }

        protected override IClientContextSupervisor CreateClientCache(Uri inputAddress, IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor)
        {
            return new ClientContextSupervisor(new QueueClientContextFactory(messagingFactoryContextSupervisor, namespaceContextSupervisor,
                MessagingFactoryPipeConfigurator.Build(), NamespacePipeConfigurator.Build(), _settings));
        }
    }
}