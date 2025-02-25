#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using Transports;
    using Util;


    public class SqlReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        ISqlReceiveEndpointConfiguration,
        ISqlReceiveEndpointConfigurator
    {
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$", RegexOptions.Compiled);

        readonly IBuildPipeConfigurator<ClientContext> _clientConfigurator;
        readonly ISqlEndpointConfiguration _endpointConfiguration;
        readonly ISqlHostConfiguration _hostConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly SqlReceiveSettings _settings;

        public SqlReceiveEndpointConfiguration(ISqlHostConfiguration hostConfiguration, SqlReceiveSettings settings,
            ISqlEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;

            _endpointConfiguration = endpointConfiguration;

            _clientConfigurator = new PipeConfigurator<ClientContext>();

            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public ReceiveSettings Settings => _settings;

        public override Uri HostAddress => _hostConfiguration.HostAddress;
        public override Uri InputAddress => _inputAddress.Value;

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateDbReceiveEndpointContext();
        }

        ISqlTopologyConfiguration ISqlEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public void Build(IHost host)
        {
            var context = CreateDbReceiveEndpointContext();

            _clientConfigurator.UseFilter(new ConfigureSqlTopologyFilter<ReceiveSettings>(_settings, context.BrokerTopology, context));

            if (_hostConfiguration.DeployTopologyOnly)
                _clientConfigurator.UseFilter(new TransportReadyFilter<ClientContext>(context));
            else
            {
                if (_settings.PurgeOnStartup)
                    _clientConfigurator.UseFilter(new PurgeOnStartupFilter(_settings.QueueName));

                _clientConfigurator.UseFilter(new ReceiveEndpointDependencyFilter<ClientContext>(context));
                _clientConfigurator.UseFilter(new SqlConsumerFilter(context));
            }

            IPipe<ClientContext> clientPipe = _clientConfigurator.Build();

            var transport = new ReceiveTransport<ClientContext>(_hostConfiguration, context, () => context.ClientContextSupervisor, clientPipe);

            if (IsBusEndpoint && _hostConfiguration.DeployPublishTopology)
            {
                var publishTopology = _hostConfiguration.Topology.PublishTopology;

                var brokerTopology = publishTopology.GetPublishBrokerTopology();

                transport.PreStartPipe = new ConfigureSqlTopologyFilter<IPublishTopology>(publishTopology, brokerTopology).ToPipe();
            }

            var receiveEndpoint = new ReceiveEndpoint(transport, context);

            var queueName = _settings.QueueName ?? NewId.Next().ToString(FormatUtil.Formatter);

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            if (!IsValidEntityName(_settings.QueueName))
                yield return this.Failure(_settings.QueueName, "Must be a valid queue name");

            if (_settings.PurgeOnStartup)
                yield return this.Warning(_settings.QueueName, "Existing messages will be purged on service start");

            if (_settings.MaintenanceBatchSize <= 0)
                yield return this.Failure(_settings.QueueName, nameof(_settings.MaintenanceBatchSize), "Must be >= 1");

            if (_settings.UnlockDelay.HasValue && _settings.UnlockDelay < TimeSpan.Zero)
                yield return this.Failure(_settings.QueueName, nameof(_settings.UnlockDelay), "Must be > TimeSpan.Zero");

            foreach (var result in base.Validate())
                yield return result.WithParentKey(_settings.QueueName);
        }

        public TimeSpan? AutoDeleteOnIdle
        {
            set
            {
                _settings.AutoDeleteOnIdle = value;

                Changed("AutoDelete");
            }
        }

        public TimeSpan PollingInterval
        {
            set => _settings.PollingInterval = value;
        }

        public TimeSpan LockDuration
        {
            set => _settings.LockDuration = value;
        }

        public TimeSpan MaxLockDuration
        {
            set => _settings.MaxLockDuration = value;
        }

        public int? MaxDeliveryCount
        {
            set => _settings.MaxDeliveryCount = value;
        }

        public bool PurgeOnStartup
        {
            set => _settings.PurgeOnStartup = value;
        }

        public int MaintenanceBatchSize
        {
            set => _settings.MaintenanceBatchSize = value;
        }

        public bool DeadLetterExpiredMessages
        {
            set => _settings.DeadLetterExpiredMessages = value;
        }

        public void Subscribe(string topicName, Action<ISqlTopicSubscriptionConfigurator>? callback)
        {
            if (topicName == null)
                throw new ArgumentNullException(nameof(topicName));

            _endpointConfiguration.Topology.Consume.Subscribe(topicName, callback);
        }

        public TimeSpan? UnlockDelay
        {
            set => _settings.UnlockDelay = value;
        }

        public int ConcurrentDeliveryLimit
        {
            set => _settings.ConcurrentDeliveryLimit = value;
        }

        public void Subscribe<T>(Action<ISqlTopicSubscriptionConfigurator>? callback)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Subscribe(callback);
        }

        public void SetReceiveMode(SqlReceiveMode mode, int? concurrentDeliveryLimit = default)
        {
            if (concurrentDeliveryLimit != null)
                _settings.ConcurrentDeliveryLimit = concurrentDeliveryLimit.Value;

            _settings.ReceiveMode = mode;
        }

        public void ConfigureClient(Action<IPipeConfigurator<ClientContext>>? configure)
        {
            configure?.Invoke(_clientConfigurator);
        }

        static bool IsValidEntityName(string name)
        {
            return _regex.Match(name).Success;
        }

        SqlReceiveEndpointContext CreateDbReceiveEndpointContext()
        {
            var builder = new SqlReceiveEndpointBuilder(_hostConfiguration, this);

            ApplySpecifications(builder);

            return builder.CreateReceiveEndpointContext();
        }

        Uri FormatInputAddress()
        {
            return _settings.GetInputAddress(_hostConfiguration.HostAddress);
        }

        protected override bool IsAlreadyConfigured()
        {
            return _inputAddress.IsValueCreated || base.IsAlreadyConfigured();
        }
    }
}
