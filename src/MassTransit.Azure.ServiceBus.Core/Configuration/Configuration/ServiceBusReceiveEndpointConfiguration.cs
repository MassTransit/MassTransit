namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using GreenPipes;
    using Pipeline;
    using Settings;
    using Topology.Configuration;
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
        readonly Lazy<Uri> _inputAddress;

        public ServiceBusReceiveEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration, ReceiveEndpointSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, settings, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
            _settings = settings;

            _settings.QueueConfigurator.BasePath = hostConfiguration.BasePath;

            SubscribeMessageTopics = true;

            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public bool SubscribeMessageTopics { get; set; }

        public ReceiveSettings Settings => _settings;

        public override Uri HostAddress => _hostConfiguration.HostAddress;

        public override Uri InputAddress => _inputAddress.Value;

        IServiceBusTopologyConfiguration IServiceBusEndpointConfiguration.Topology => _endpointConfiguration.Topology;

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

        public int MaxSizeInMegabytes
        {
            set => _settings.QueueConfigurator.MaxSizeInMB = value;
        }

        public bool RequiresDuplicateDetection
        {
            set => _settings.QueueConfigurator.RequiresDuplicateDetection = value;
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

        Uri FormatInputAddress()
        {
            return _settings.GetInputAddress(_hostConfiguration.HostAddress, _settings.Path);
        }

        protected override bool IsAlreadyConfigured()
        {
            return _inputAddress.IsValueCreated || base.IsAlreadyConfigured();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return _settings.QueueConfigurator.Validate()
                .Concat(base.Validate());
        }

        public void Build(IServiceBusHostControl host)
        {
            var builder = new ServiceBusReceiveEndpointBuilder(host, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            NamespacePipeConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointContext.BrokerTopology,
                _settings.RemoveSubscriptions, host.Stopping));

            CreateReceiveEndpoint(host, receiveEndpointContext);
        }

        protected override IErrorTransport CreateErrorTransport(IServiceBusHostControl host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings.QueueConfigurator);

            return new BrokeredMessageErrorTransport(CreateSendEndpointContextSupervisor(host, settings));
        }

        protected override IDeadLetterTransport CreateDeadLetterTransport(IServiceBusHostControl host)
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.QueueConfigurator);

            return new BrokeredMessageDeadLetterTransport(CreateSendEndpointContextSupervisor(host, settings));
        }

        protected override IClientContextSupervisor CreateClientCache(IMessagingFactoryContextSupervisor messagingFactoryContextSupervisor,
            INamespaceContextSupervisor namespaceContextSupervisor)
        {
            return new ClientContextSupervisor(new QueueClientContextFactory(messagingFactoryContextSupervisor, namespaceContextSupervisor,
                MessagingFactoryPipeConfigurator.Build(), NamespacePipeConfigurator.Build(), _settings));
        }
    }
}
