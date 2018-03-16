namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Context;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using MassTransit.Pipeline;
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
            InputAddress = new Uri(hostConfiguration.Host.Address, $"{settings.Name}");

            SubscribeMessageTopics = true;
        }

        public override IReceiveEndpoint CreateReceiveEndpoint(string endpointName, IReceiveTransport receiveTransport, IReceivePipe receivePipe,
            ReceiveEndpointContext topology)
        {
            var receiveEndpoint = new ReceiveEndpoint(receiveTransport, receivePipe, topology);

            _hostConfiguration.Host.AddReceiveEndpoint(endpointName, receiveEndpoint);

            return receiveEndpoint;
        }

        public IServiceBusBusConfiguration BusConfiguration => _hostConfiguration.BusConfiguration;

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

            var receivePipe = CreateReceivePipe();

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            NamespacePipeConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointContext.BrokerTopology,
                _settings.RemoveSubscriptions));

            return CreateReceiveEndpoint(builder, receivePipe, receiveEndpointContext);
        }

        protected override IErrorTransport CreateErrorTransport(ServiceBusHost host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings.QueueConfigurator);

            return new BrokeredMessageErrorTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport(ServiceBusHost host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.QueueConfigurator);

            return new BrokeredMessageDeadLetterTransport(CreateSendEndpointContextCache(host, settings));
        }

        protected override IPipeContextFactory<SendEndpointContext> CreateSendEndpointContextFactory(IServiceBusHost host, SendSettings settings,
            IPipe<NamespaceContext> namespacePipe)
        {
            return new QueueSendEndpointContextFactory(host.MessagingFactoryCache, host.NamespaceCache, Pipe.Empty<MessagingFactoryContext>(), namespacePipe,
                settings);
        }

        protected override IClientCache CreateClientCache(Uri inputAddress, IMessagingFactoryCache messagingFactoryCache, INamespaceCache namespaceCache)
        {
            return new ClientCache(inputAddress,
                new QueueClientContextFactory(messagingFactoryCache, namespaceCache, MessagingFactoryPipeConfigurator.Build(),
                    NamespacePipeConfigurator.Build(), _settings));
        }
    }
}