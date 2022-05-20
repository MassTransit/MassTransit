namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Middleware;
    using Topology;
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

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateServiceBusReceiveEndpointContext();
        }

        IServiceBusTopologyConfiguration IServiceBusEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override IEnumerable<ValidationResult> Validate()
        {
            return _settings.QueueConfigurator.Validate()
                .Concat(base.Validate());
        }

        public void Build(IHost host)
        {
            var context = CreateServiceBusReceiveEndpointContext();

            ClientPipeConfigurator.UseFilter(new ConfigureServiceBusTopologyFilter<ReceiveSettings>(_settings, context.BrokerTopology,
                _settings.RemoveSubscriptions, context));

            var errorTransport = CreateErrorTransport();
            var deadLetterTransport = CreateDeadLetterTransport();

            context.GetOrAddPayload(() => deadLetterTransport);
            context.GetOrAddPayload(() => errorTransport);

            CreateReceiveEndpoint(host, context);
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

        public long MaxSizeInMegabytes
        {
            set => _settings.QueueConfigurator.MaxSizeInMegabytes = value;
        }

        public long MaxMessageSizeInKilobytes
        {
            set => _settings.QueueConfigurator.MaxMessageSizeInKilobytes = value;
        }

        public bool RequiresDuplicateDetection
        {
            set => _settings.QueueConfigurator.RequiresDuplicateDetection = value;
        }

        public bool RemoveSubscriptions
        {
            set => _settings.RemoveSubscriptions = value;
        }

        public void Subscribe(string topicName, string subscriptionName, Action<IServiceBusSubscriptionConfigurator> callback)
        {
            _endpointConfiguration.Topology.Consume.Subscribe(topicName, subscriptionName, callback);
        }

        public void Subscribe<T>(string subscriptionName, Action<IServiceBusSubscriptionConfigurator> callback)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Subscribe(subscriptionName, callback);
        }

        public override void SelectBasicTier()
        {
            base.SelectBasicTier();

            ConfigureConsumeTopology = false;
        }

        ServiceBusReceiveEndpointContext CreateServiceBusReceiveEndpointContext()
        {
            var builder = new ServiceBusReceiveEndpointBuilder(_hostConfiguration, this);

            ApplySpecifications(builder);

            return builder.CreateReceiveEndpointContext();
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

            return new ServiceBusQueueErrorTransport(_hostConfiguration.ConnectionContextSupervisor, settings);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var settings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings.QueueConfigurator);

            return new ServiceBusQueueDeadLetterTransport(_hostConfiguration.ConnectionContextSupervisor, settings);
        }
    }
}
