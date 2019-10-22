namespace MassTransit.ActiveMqTransport.Configuration
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


    public class ActiveMqReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IActiveMqReceiveEndpointConfiguration,
        IActiveMqReceiveEndpointConfigurator
    {
        readonly IBuildPipeConfigurator<ConnectionContext> _connectionConfigurator;
        readonly IActiveMqEndpointConfiguration _endpointConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly IBuildPipeConfigurator<SessionContext> _sessionConfigurator;
        readonly QueueReceiveSettings _settings;
        readonly IActiveMqHostConfiguration _hostConfiguration;

        public ActiveMqReceiveEndpointConfiguration(IActiveMqHostConfiguration hostConfiguration, QueueReceiveSettings settings,
            IActiveMqEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _settings = settings;

            BindMessageTopics = true;

            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;

            _connectionConfigurator = new PipeConfigurator<ConnectionContext>();
            _sessionConfigurator = new PipeConfigurator<SessionContext>();

            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public bool BindMessageTopics { get; set; }
        public ReceiveSettings Settings => _settings;
        public override Uri HostAddress => _hostConfiguration.HostAddress;
        public override Uri InputAddress => _inputAddress.Value;
        IActiveMqTopologyConfiguration IActiveMqEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public void Build(IActiveMqHostControl host)
        {
            var builder = new ActiveMqReceiveEndpointBuilder(host, this);

            ApplySpecifications(builder);

            var receiveEndpointContext = builder.CreateReceiveEndpointContext();

            _sessionConfigurator.UseFilter(new ConfigureTopologyFilter<ReceiveSettings>(_settings, receiveEndpointContext.BrokerTopology));

            IAgent consumerAgent;
            if (_hostConfiguration.DeployTopologyOnly)
            {
                var transportReadyFilter = new TransportReadyFilter<SessionContext>(receiveEndpointContext);
                _sessionConfigurator.UseFilter(transportReadyFilter);

                consumerAgent = transportReadyFilter;
            }
            else
            {
                var consumerFilter = new ActiveMqConsumerFilter(receiveEndpointContext);

                _sessionConfigurator.UseFilter(consumerFilter);

                consumerAgent = consumerFilter;
            }

            IFilter<ConnectionContext> sessionFilter = new ReceiveSessionFilter(_sessionConfigurator.Build(), host);

            _connectionConfigurator.UseFilter(sessionFilter);

            var transport = new ActiveMqReceiveTransport(host, _settings, _connectionConfigurator.Build(), receiveEndpointContext);
            transport.Add(consumerAgent);

            var receiveEndpoint = new ReceiveEndpoint(transport, receiveEndpointContext);

            var queueName = _settings.EntityName ?? NewId.Next().ToString(FormatUtil.Formatter);

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            var queueName = $"{_settings.EntityName}";

            if (!ActiveMqEntityNameValidator.Validator.IsValidEntityName(_settings.EntityName))
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

        public bool AutoDelete
        {
            set
            {
                _settings.AutoDelete = value;

                Changed("AutoDelete");
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

        public void ConfigureSession(Action<IPipeConfigurator<SessionContext>> configure)
        {
            configure?.Invoke(_sessionConfigurator);
        }

        public void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure)
        {
            configure?.Invoke(_connectionConfigurator);
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
