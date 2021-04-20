namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using GreenPipes;
    using Pipeline;
    using Settings;
    using Topology;
    using Transport;
    using Transports;


    public class ServiceBusReceiveEndpointConfiguration :
        ServiceBusEntityReceiveEndpointConfiguration,
        IServiceBusReceiveEndpointConfiguration,
        IServiceBusReceiveEndpointConfigurator
    {
        readonly IServiceBusEndpointConfiguration _endpointConfiguration;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly ReceiveEndpointSettings _settings;

        public ServiceBusReceiveEndpointConfiguration(IServiceBusHostConfiguration hostConfiguration, ReceiveEndpointSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, settings, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;
            _settings = settings;

            _settings.QueueConfigurator.BasePath = hostConfiguration.BasePath;

            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public ReceiveSettings Settings => _settings;

        public override Uri HostAddress => _hostConfiguration.HostAddress;

        public override Uri InputAddress => _inputAddress.Value;

        IServiceBusTopologyConfiguration IServiceBusEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override IEnumerable<ValidationResult> Validate()
        {
            return _settings.QueueConfigurator.Validate()
                .Concat(base.Validate());
        }

        public void Build(IHost host)
        {
            var builder = new ServiceBusReceiveEndpointBuilder(_hostConfiguration, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            ClientPipeConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointContext.BrokerTopology,
                _settings.RemoveSubscriptions, _hostConfiguration.ConnectionContextSupervisor.Stopping));

            var errorTransport = CreateErrorTransport();
            var deadLetterTransport = CreateDeadLetterTransport();

            receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);
            receiveEndpointContext.GetOrAddPayload(() => errorTransport);

            CreateReceiveEndpoint(host, receiveEndpointContext);
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

            ConfigureConsumeTopology = false;
        }

        Uri FormatInputAddress()
        {
            return _settings.GetInputAddress(_hostConfiguration.HostAddress, _settings.Path);
        }

        protected override bool IsAlreadyConfigured()
        {
            return _inputAddress.IsValueCreated || base.IsAlreadyConfigured();
        }

        IErrorTransport CreateErrorTransport()
        {
            var settings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings.QueueConfigurator);

            return new BrokeredMessageErrorTransport(_hostConfiguration.ConnectionContextSupervisor, settings);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.QueueConfigurator);

            return new BrokeredMessageDeadLetterTransport(_hostConfiguration.ConnectionContextSupervisor, settings);
        }
    }
}
