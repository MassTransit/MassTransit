namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using RabbitMQ.Client;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IRabbitMqReceiveEndpointConfiguration,
        IRabbitMqReceiveEndpointConfigurator
    {
        readonly IBuildPipeConfigurator<ConnectionContext> _connectionConfigurator;
        readonly IRabbitMqEndpointConfiguration _endpointConfiguration;
        readonly IRabbitMqHostConfiguration _hostConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly IBuildPipeConfigurator<ChannelContext> _channelConfigurator;
        readonly RabbitMqReceiveSettings _settings;

        public RabbitMqReceiveEndpointConfiguration(IRabbitMqHostConfiguration hostConfiguration, RabbitMqReceiveSettings settings,
            IRabbitMqEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;

            _endpointConfiguration = endpointConfiguration;

            _connectionConfigurator = new PipeConfigurator<ConnectionContext>();
            _channelConfigurator = new PipeConfigurator<ChannelContext>();

            _inputAddress = new Lazy<Uri>(FormatInputAddress);

            if (settings.QueueName == RabbitMqExchangeNames.ReplyTo)
            {
                settings.ExchangeName = null;
                settings.BindQueue = true;
                settings.NoAck = true;
            }
        }

        public ReceiveSettings Settings => _settings;

        public override Uri HostAddress => _hostConfiguration.HostAddress;
        public override Uri InputAddress => _inputAddress.Value;

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateRabbitMqReceiveEndpointContext();
        }

        IRabbitMqTopologyConfiguration IRabbitMqEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public void Build(IHost host)
        {
            var context = CreateRabbitMqReceiveEndpointContext();

            _channelConfigurator.UseFilter(new ConfigureRabbitMqTopologyFilter<ReceiveSettings>(_settings, context.BrokerTopology));

            if (_hostConfiguration.DeployTopologyOnly)
                _channelConfigurator.UseFilter(new TransportReadyFilter<ChannelContext>(context));
            else
            {
                if (_settings.PurgeOnStartup)
                    _channelConfigurator.UseFilter(new PurgeOnStartupFilter(_settings.QueueName));

                _channelConfigurator.UseFilter(new PrefetchCountFilter(_settings.PrefetchCount));
                _channelConfigurator.UseFilter(new ReceiveEndpointDependencyFilter<ChannelContext>(context));
                _channelConfigurator.UseFilter(new RabbitMqConsumerFilter(context));
            }

            IPipe<ChannelContext> channelPipe = _channelConfigurator.Build();

            var transport = new ReceiveTransport<ChannelContext>(_hostConfiguration, context, () => context.ChannelContextSupervisor, channelPipe);

            if (IsBusEndpoint && _hostConfiguration.DeployPublishTopology)
            {
                var publishTopology = _hostConfiguration.Topology.PublishTopology;

                var brokerTopology = publishTopology.GetPublishBrokerTopology();

                transport.PreStartPipe = new ConfigureRabbitMqTopologyFilter<IPublishTopology>(publishTopology, brokerTopology).ToPipe();
            }

            var receiveEndpoint = new ReceiveEndpoint(transport, context);

            var queueName = _settings.QueueName ?? NewId.Next().ToString(FormatUtil.Formatter);

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            var queueName = $"{_settings.QueueName}";

            if (!RabbitMqEntityNameValidator.Validator.IsValidEntityName(_settings.QueueName))
                yield return this.Failure(queueName, "must be a valid queue name");

            if (_settings.PurgeOnStartup)
                yield return this.Warning(queueName, "Existing messages in the queue will be purged on service start");

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

        public bool Exclusive
        {
            set
            {
                _settings.Exclusive = value;

                Changed("Exclusive");
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

        public string ExchangeType
        {
            set => _settings.ExchangeType = value;
        }

        public bool PurgeOnStartup
        {
            set => _settings.PurgeOnStartup = value;
        }

        public int ConsumerPriority
        {
            set => _settings.ConsumerPriority = value;
        }

        public bool ExclusiveConsumer
        {
            set => _settings.ExclusiveConsumer = value;
        }

        public void Stream(Action<IRabbitMqStreamConfigurator> callback = null)
        {
            _settings.QueueArguments[Headers.XQueueType] = "stream";

            var configurator = new RabbitMqStreamConfigurator(_settings);

            callback?.Invoke(configurator);
        }

        public void Stream(string consumerTag, Action<IRabbitMqStreamConfigurator> callback = null)
        {
            if (string.IsNullOrWhiteSpace(consumerTag))
                throw new ArgumentNullException(nameof(consumerTag));

            _settings.ConsumerTag = consumerTag;

            Stream(callback);
        }

        public bool Lazy
        {
            set => _settings.Lazy = value;
        }

        public bool BindQueue
        {
            set => _settings.BindQueue = value;
        }

        public TimeSpan? QueueExpiration
        {
            set => _settings.QueueExpiration = value;
        }

        public bool SingleActiveConsumer
        {
            set => _settings.SingleActiveConsumer = value;
        }

        public string DeadLetterExchange
        {
            set => SetQueueArgument(Headers.XDeadLetterExchange, value);
        }

        public void SetQueueArgument(string key, object value)
        {
            _settings.SetQueueArgument(key, value);
        }

        public void SetQueueArgument(string key, TimeSpan value)
        {
            _settings.SetQueueArgument(key, value);
        }

        public void SetExchangeArgument(string key, object value)
        {
            _settings.SetExchangeArgument(key, value);
        }

        public void SetExchangeArgument(string key, TimeSpan value)
        {
            _settings.SetExchangeArgument(key, value);
        }

        public void EnablePriority(byte maxPriority)
        {
            _settings.EnablePriority(maxPriority);
        }

        public void SetQuorumQueue(int? replicationFactor = default)
        {
            _settings.SetQuorumQueue(replicationFactor);
        }

        public void SetDeliveryAcknowledgementTimeout(TimeSpan timeSpan)
        {
            if (timeSpan <= TimeSpan.Zero)
                throw new ArgumentException("The RabbitMQ consumer timeout must be > 0");

            SetQueueArgument("x-consumer-timeout", (long)timeSpan.TotalMilliseconds);
        }

        public void SetDeliveryAcknowledgementTimeout(int? d = null, int? h = null, int? m = null, int? s = null, int? ms = null)
        {
            var value = new TimeSpan(d ?? 0, h ?? 0, m ?? 0, s ?? 0, ms ?? 0);

            SetDeliveryAcknowledgementTimeout(value);
        }

        public void Bind(string exchangeName, Action<IRabbitMqExchangeToExchangeBindingConfigurator> callback)
        {
            if (exchangeName == null)
                throw new ArgumentNullException(nameof(exchangeName));

            _endpointConfiguration.Topology.Consume.Bind(exchangeName, callback);
        }

        public void Bind<T>(Action<IRabbitMqExchangeBindingConfigurator> callback)
            where T : class
        {
            _endpointConfiguration.Topology.Consume.GetMessageTopology<T>().Bind(callback);
        }

        public void BindDeadLetterQueue(string exchangeName, string queueName, Action<IRabbitMqQueueBindingConfigurator> configure)
        {
            _endpointConfiguration.Topology.Consume.BindQueue(exchangeName, queueName, configure);

            DeadLetterExchange = exchangeName;
        }

        public void ConfigureChannel(Action<IPipeConfigurator<ChannelContext>> configure)
        {
            configure?.Invoke(_channelConfigurator);
        }

        public void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure)
        {
            configure?.Invoke(_connectionConfigurator);
        }

        public void OverrideConsumerTag(string consumerTag)
        {
            _settings.ConsumerTag = consumerTag;
        }

        RabbitMqReceiveEndpointContext CreateRabbitMqReceiveEndpointContext()
        {
            var builder = new RabbitMqReceiveEndpointBuilder(_hostConfiguration, this);

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
