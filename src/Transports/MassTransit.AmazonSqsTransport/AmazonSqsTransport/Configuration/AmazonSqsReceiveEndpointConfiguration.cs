﻿namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using Topology;
    using Transports;
    using Util;


    public class AmazonSqsReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IAmazonSqsReceiveEndpointConfiguration,
        IAmazonSqsReceiveEndpointConfigurator
    {
        static readonly TimeSpan MaxAllowedVisibilityTimeout = TimeSpan.FromHours(12);

        readonly IBuildPipeConfigurator<ClientContext> _clientConfigurator;
        readonly IBuildPipeConfigurator<ConnectionContext> _connectionConfigurator;
        readonly IAmazonSqsEndpointConfiguration _endpointConfiguration;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly QueueReceiveSettings _settings;

        public AmazonSqsReceiveEndpointConfiguration(IAmazonSqsHostConfiguration hostConfiguration, QueueReceiveSettings settings,
            IAmazonSqsEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _settings = settings;

            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;

            _connectionConfigurator = new PipeConfigurator<ConnectionContext>();
            _clientConfigurator = new PipeConfigurator<ClientContext>();

            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public ReceiveSettings Settings => _settings;
        public override Uri HostAddress => _hostConfiguration.HostAddress;
        public override Uri InputAddress => _inputAddress.Value;

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateSqsReceiveEndpointContext();
        }

        IAmazonSqsTopologyConfiguration IAmazonSqsEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public void Build(IHost host)
        {
            var context = CreateSqsReceiveEndpointContext();

            _clientConfigurator.UseFilter(new ConfigureAmazonSqsTopologyFilter<ReceiveSettings>(_settings, context.BrokerTopology, context));

            if (_hostConfiguration.DeployTopologyOnly)
                _clientConfigurator.UseFilter(new TransportReadyFilter<ClientContext>(context));
            else
            {
                if (_settings.PurgeOnStartup)
                    _clientConfigurator.UseFilter(new PurgeOnStartupFilter(_settings.EntityName));

                _clientConfigurator.UseFilter(new ReceiveEndpointDependencyFilter<ClientContext>(context));
                _clientConfigurator.UseFilter(new AmazonSqsConsumerFilter(context));
            }

            IPipe<ClientContext> clientPipe = _clientConfigurator.Build();

            var transport = new ReceiveTransport<ClientContext>(_hostConfiguration, context,
                () => context.ClientContextSupervisor, clientPipe);

            if (IsBusEndpoint && _hostConfiguration.DeployPublishTopology)
            {
                var publishTopology = _hostConfiguration.Topology.PublishTopology;

                var brokerTopology = publishTopology.GetPublishBrokerTopology();

                transport.PreStartPipe = new ConfigureAmazonSqsTopologyFilter<IPublishTopology>(publishTopology, brokerTopology).ToPipe();
            }

            var receiveEndpoint = new ReceiveEndpoint(transport, context);

            var queueName = _settings.EntityName ?? NewId.Next().ToString(FormatUtil.Formatter);

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            if (_settings.PrefetchCount <= 0)
                yield return this.Failure("PrefetchCount", "must be >= 1");

            var queueName = $"{_settings.EntityName}";

            if (!AmazonSqsEntityNameValidator.Validator.IsValidEntityName(_settings.EntityName))
                yield return this.Failure(queueName, "must be a valid queue name");

            if (_settings.PurgeOnStartup)
                yield return this.Warning(queueName, "Existing messages in the queue will be purged on service start");

            var visibilityTimeout = TimeSpan.FromSeconds(_settings.VisibilityTimeout);
            if (_settings.MaxVisibilityTimeout < visibilityTimeout)
            {
                yield return this.Failure("MaxVisibilityTimeout", "Must be greater than or equal to VisibilityTimeout");
            }

            foreach (var result in base.Validate())
                yield return result.WithParentKey(queueName);
        }

        public bool Durable
        {
            set
            {
                _settings.Durable = value;

                Changed("Durable");
            }
        }

        public bool AutoDelete
        {
            set
            {
                _settings.AutoDelete = value;

                Changed("AutoDelete");
            }
        }

        public int ConcurrentDeliveryLimit
        {
            set => _settings.ConcurrentDeliveryLimit = value;
        }

        public ushort WaitTimeSeconds
        {
            set => _settings.WaitTimeSeconds = value;
        }

        public bool PurgeOnStartup
        {
            set => _settings.PurgeOnStartup = value;
        }

        public IDictionary<string, object> QueueAttributes => _settings.QueueAttributes;
        public IDictionary<string, object> QueueSubscriptionAttributes => _settings.QueueSubscriptionAttributes;
        public IDictionary<string, string> QueueTags => _settings.QueueTags;

        public void Subscribe(string topicName, Action<IAmazonSqsTopicSubscriptionConfigurator> configure = null)
        {
            if (topicName == null)
                throw new ArgumentNullException(nameof(topicName));

            _endpointConfiguration.Topology.Consume.Bind(topicName, configure);
        }

        public int RedeliverVisibilityTimeout
        {
            set => _settings.RedeliverVisibilityTimeout = value;
        }

        public TimeSpan MaxVisibilityTimeout
        {
            set => _settings.MaxVisibilityTimeout = value > MaxAllowedVisibilityTimeout ? MaxAllowedVisibilityTimeout : value;
        }

        public void Subscribe<T>(Action<IAmazonSqsTopicSubscriptionConfigurator> configure = null)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Subscribe(configure);
        }

        public void ConfigureClient(Action<IPipeConfigurator<ClientContext>> configure)
        {
            configure?.Invoke(_clientConfigurator);
        }

        public void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure)
        {
            configure?.Invoke(_connectionConfigurator);
        }

        public void DisableMessageOrdering()
        {
            _settings.IsOrdered = false;
        }

        SqsReceiveEndpointContext CreateSqsReceiveEndpointContext()
        {
            var builder = new AmazonSqsReceiveEndpointBuilder(_hostConfiguration, this);

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
