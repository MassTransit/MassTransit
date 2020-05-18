namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using MassTransit.Configuration;
    using MassTransit.Pipeline.Filters;
    using Pipeline;
    using Topology;
    using Topology.Settings;
    using Transport;
    using Transports;
    using Util;


    public class AmazonSqsReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IAmazonSqsReceiveEndpointConfiguration,
        IAmazonSqsReceiveEndpointConfigurator
    {
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
        IAmazonSqsTopologyConfiguration IAmazonSqsEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public void Build(IAmazonSqsHostControl host)
        {
            var builder = new AmazonSqsReceiveEndpointBuilder(host, _hostConfiguration.Settings, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            _clientConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointContext.BrokerTopology));

            IAgent consumerAgent;
            if (_hostConfiguration.DeployTopologyOnly)
            {
                var transportReadyFilter = new TransportReadyFilter<ClientContext>(receiveEndpointContext);
                _clientConfigurator.UseFilter(transportReadyFilter);

                consumerAgent = transportReadyFilter;
            }
            else
            {
                if (_settings.PurgeOnStartup)
                    _clientConfigurator.UseFilter(new PurgeOnStartupFilter(_settings.EntityName));

                var consumerFilter = new AmazonSqsConsumerFilter(receiveEndpointContext);

                _clientConfigurator.UseFilter(consumerFilter);

                consumerAgent = consumerFilter;
            }

            IFilter<ConnectionContext> clientFilter = new ReceiveClientFilter(_clientConfigurator.Build());

            _connectionConfigurator.UseFilter(clientFilter);

            var transport = new SqsReceiveTransport(host, _settings, _connectionConfigurator.Build(), receiveEndpointContext);
            transport.Add(consumerAgent);

            var receiveEndpoint = new ReceiveEndpoint(transport, receiveEndpointContext);

            var queueName = _settings.EntityName ?? NewId.Next().ToString(FormatUtil.Formatter);

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            var queueName = $"{_settings.EntityName}";

            if (!AmazonSqsEntityNameValidator.Validator.IsValidEntityName(_settings.EntityName))
                yield return this.Failure(queueName, "must be a valid queue name");

            if (_settings.PurgeOnStartup)
                yield return this.Warning(queueName, "Existing messages in the queue will be purged on service start");

            foreach (var result in base.Validate())
                yield return result.WithParentKey(queueName);
        }

        public bool SubscribeMessageTopics
        {
            set => ConfigureConsumeTopology = value;
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

        public ushort PrefetchCount
        {
            set => _settings.PrefetchCount = value;
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

        public void Subscribe(string topicName, Action<ITopicSubscriptionConfigurator> configure = null)
        {
            if (topicName == null)
                throw new ArgumentNullException(nameof(topicName));

            _endpointConfiguration.Topology.Consume.Bind(topicName, configure);
        }

        public void Subscribe<T>(Action<ITopicSubscriptionConfigurator> configure = null)
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

        public AmazonSqsEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return _settings.GetEndpointAddress(hostAddress);
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
