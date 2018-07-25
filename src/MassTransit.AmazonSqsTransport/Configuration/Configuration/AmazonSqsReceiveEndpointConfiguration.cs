namespace MassTransit.AmazonSqsTransport.Configuration.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Builders;
    using GreenPipes.Configurators;
    using MassTransit.Configuration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Filters;
    using Pipeline;
    using Topology;
    using Topology.Settings;
    using Transport;
    using Transports;


    public class AmazonSqsReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IAmazonSqsReceiveEndpointConfiguration,
        IAmazonSqsReceiveEndpointConfigurator
    {
        readonly IBuildPipeConfigurator<ConnectionContext> _connectionConfigurator;
        readonly IAmazonSqsEndpointConfiguration _endpointConfiguration;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly IBuildPipeConfigurator<ModelContext> _modelConfigurator;
        readonly AmazonSqsReceiveSettings _settings;

        public AmazonSqsReceiveEndpointConfiguration(IAmazonSqsHostConfiguration hostConfiguration, string queueName,
            IAmazonSqsEndpointConfiguration endpointConfiguration)
            : this(hostConfiguration, endpointConfiguration)
        {
            BindMessageTopics = true;

            _settings = new AmazonSqsReceiveSettings(queueName, true, false);
        }

        public AmazonSqsReceiveEndpointConfiguration(IAmazonSqsHostConfiguration hostConfiguration, AmazonSqsReceiveSettings settings,
            IAmazonSqsEndpointConfiguration endpointConfiguration)
            : this(hostConfiguration, endpointConfiguration)
        {
            _settings = settings;
        }

        AmazonSqsReceiveEndpointConfiguration(IAmazonSqsHostConfiguration hostConfiguration, IAmazonSqsEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;

            _connectionConfigurator = new PipeConfigurator<ConnectionContext>();
            _modelConfigurator = new PipeConfigurator<ModelContext>();

            HostAddress = hostConfiguration.Host.Address;

            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public IAmazonSqsReceiveEndpointConfigurator Configurator => this;

        public IAmazonSqsBusConfiguration BusConfiguration => _hostConfiguration.BusConfiguration;

        public IAmazonSqsHostControl Host => _hostConfiguration.Host;

        public bool BindMessageTopics { get; set; }

        public ReceiveSettings Settings => _settings;

        public override Uri HostAddress { get; }

        public override Uri InputAddress => _inputAddress.Value;

        public override IReceiveEndpoint CreateReceiveEndpoint(string endpointName, IReceiveTransport receiveTransport, IReceivePipe receivePipe,
            ReceiveEndpointContext receiveEndpointContext)
        {
            var receiveEndpoint = new ReceiveEndpoint(receiveTransport, receivePipe, receiveEndpointContext);

            _hostConfiguration.Host.AddReceiveEndpoint(endpointName, receiveEndpoint);

            return receiveEndpoint;
        }

        IAmazonSqsTopologyConfiguration IAmazonSqsEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override IReceiveEndpoint Build()
        {
            var builder = new AmazonSqsReceiveEndpointBuilder(this);

            ApplySpecifications(builder);

            var receivePipe = CreateReceivePipe();

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            _modelConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointContext.BrokerTopology));

            IAgent consumerAgent;
            if (_hostConfiguration.BusConfiguration.DeployTopologyOnly)
            {
                var transportReadyFilter = new TransportReadyFilter<ModelContext>(builder.TransportObservers, InputAddress);
                _modelConfigurator.UseFilter(transportReadyFilter);

                consumerAgent = transportReadyFilter;
            }
            else
            {
                var deadLetterTransport = CreateDeadLetterTransport();

                var errorTransport = CreateErrorTransport();

                var consumerFilter = new AmazonSqsConsumerFilter(receivePipe, builder.ReceiveObservers, builder.TransportObservers, receiveEndpointContext,
                    deadLetterTransport, errorTransport);

                _modelConfigurator.UseFilter(consumerFilter);

                consumerAgent = consumerFilter;
            }

            IFilter<ConnectionContext> sessionFilter = new ReceiveModelFilter(_modelConfigurator.Build(), _hostConfiguration.Host);

            _connectionConfigurator.UseFilter(sessionFilter);

            var transport = new AmazonSqsReceiveTransport(_hostConfiguration.Host, _settings, _connectionConfigurator.Build(), receiveEndpointContext,
                builder.ReceiveObservers, builder.TransportObservers);

            transport.Add(consumerAgent);

            return CreateReceiveEndpoint(_settings.EntityName ?? NewId.Next().ToString(), transport, receivePipe, receiveEndpointContext);
        }

        IAmazonSqsHost IAmazonSqsReceiveEndpointConfigurator.Host => Host;

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
            set
            {
                _settings.PrefetchCount = value;
                Changed("PrefetchCount");
            }
        }

        public ushort WaitTimeSeconds
        {
            set
            {
                _settings.WaitTimeSeconds = value;
                Changed("WaitTimeSeconds");
            }
        }

        public bool Lazy
        {
            set => _settings.Lazy = value;
        }

        public void Bind(string topicName, Action<ITopicBindingConfigurator> configure = null)
        {
            if (topicName == null)
                throw new ArgumentNullException(nameof(topicName));

            _endpointConfiguration.Topology.Consume.Bind(topicName, configure);
        }

        public void Bind<T>(Action<ITopicBindingConfigurator> configure = null)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Bind(configure);
        }

        public void ConfigureSession(Action<IPipeConfigurator<ModelContext>> configure)
        {
            configure?.Invoke(_modelConfigurator);
        }

        public void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure)
        {
            configure?.Invoke(_connectionConfigurator);
        }

        Uri FormatInputAddress()
        {
            return _settings.GetInputAddress(_hostConfiguration.Host.Settings.HostAddress);
        }

        IErrorTransport CreateErrorTransport()
        {
            var errorSettings = _endpointConfiguration.Topology.Send.GetErrorSettings(_settings);
            var filter = new ConfigureTopologyFilter<ErrorSettings>(errorSettings, errorSettings.GetBrokerTopology());

            return new AmazonSqsErrorTransport(errorSettings.EntityName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var deadLetterSettings = _endpointConfiguration.Topology.Send.GetDeadLetterSettings(_settings);
            var filter = new ConfigureTopologyFilter<DeadLetterSettings>(deadLetterSettings, deadLetterSettings.GetBrokerTopology());

            return new AmazonSqsDeadLetterTransport(deadLetterSettings.EntityName, filter);
        }

        protected override bool IsAlreadyConfigured()
        {
            return _inputAddress.IsValueCreated || base.IsAlreadyConfigured();
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
    }
}
