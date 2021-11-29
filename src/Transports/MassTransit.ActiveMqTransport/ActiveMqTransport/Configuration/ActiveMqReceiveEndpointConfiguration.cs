namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using Topology;
    using Transports;
    using Util;


    public class ActiveMqReceiveEndpointConfiguration :
        ReceiveEndpointConfiguration,
        IActiveMqReceiveEndpointConfiguration,
        IActiveMqReceiveEndpointConfigurator
    {
        readonly IBuildPipeConfigurator<ConnectionContext> _connectionConfigurator;
        readonly IActiveMqEndpointConfiguration _endpointConfiguration;
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly Lazy<Uri> _inputAddress;
        readonly IBuildPipeConfigurator<SessionContext> _sessionConfigurator;
        readonly ActiveMqQueueReceiveSettings _settings;

        public ActiveMqReceiveEndpointConfiguration(IActiveMqHostConfiguration hostConfiguration, ActiveMqQueueReceiveSettings settings,
            IActiveMqEndpointConfiguration endpointConfiguration)
            : base(hostConfiguration, endpointConfiguration)
        {
            _settings = settings;

            _hostConfiguration = hostConfiguration;
            _endpointConfiguration = endpointConfiguration;

            _connectionConfigurator = new PipeConfigurator<ConnectionContext>();
            _sessionConfigurator = new PipeConfigurator<SessionContext>();

            _inputAddress = new Lazy<Uri>(FormatInputAddress);
        }

        public ReceiveSettings Settings => _settings;
        public override Uri HostAddress => _hostConfiguration.HostAddress;
        public override Uri InputAddress => _inputAddress.Value;
        IActiveMqTopologyConfiguration IActiveMqEndpointConfiguration.Topology => _endpointConfiguration.Topology;

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateActiveMqReceiveEndpointContext();
        }

        public void Build(IHost host)
        {
            var context = CreateActiveMqReceiveEndpointContext();

            _sessionConfigurator.UseFilter(new ConfigureActiveMqTopologyFilter<ReceiveSettings>(_settings, context.BrokerTopology));

            if (_hostConfiguration.DeployTopologyOnly)
                _sessionConfigurator.UseFilter(new TransportReadyFilter<SessionContext>(context));
            else
                _sessionConfigurator.UseFilter(new ActiveMqConsumerFilter(context));

            IPipe<SessionContext> sessionPipe = _sessionConfigurator.Build();

            var transport = new ReceiveTransport<SessionContext>(_hostConfiguration, context,
                () => context.SessionContextSupervisor, sessionPipe);

            var receiveEndpoint = new ReceiveEndpoint(transport, context);

            var queueName = _settings.EntityName ?? NewId.Next().ToString(FormatUtil.Formatter);

            host.AddReceiveEndpoint(queueName, receiveEndpoint);

            ReceiveEndpoint = receiveEndpoint;
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            var queueName = $"{_settings.EntityName}";

            if (!ActiveMqEntityNameValidator.Validator.IsValidEntityName(_settings.EntityName))
                yield return this.Failure(queueName, "must be a valid queue name");

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

        public void Bind(string topicName, Action<IActiveMqTopicBindingConfigurator> configure = null)
        {
            if (topicName == null)
                throw new ArgumentNullException(nameof(topicName));

            _endpointConfiguration.Topology.Consume.Bind(topicName, configure);
        }

        public void Bind<T>(Action<IActiveMqTopicBindingConfigurator> configure = null)
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

        ActiveMqReceiveEndpointContext CreateActiveMqReceiveEndpointContext()
        {
            var builder = new ActiveMqReceiveEndpointBuilder(_hostConfiguration, this);

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
