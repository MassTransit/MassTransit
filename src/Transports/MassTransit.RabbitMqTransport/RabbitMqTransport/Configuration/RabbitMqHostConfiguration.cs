namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using RabbitMQ.Client.Exceptions;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqHostConfiguration :
        BaseHostConfiguration<IRabbitMqReceiveEndpointConfiguration, IRabbitMqReceiveEndpointConfigurator>,
        IRabbitMqHostConfiguration
    {
        readonly IRabbitMqBusConfiguration _busConfiguration;
        readonly Recycle<IConnectionContextSupervisor> _connectionContext;
        readonly IRabbitMqBusTopology _topology;
        RabbitMqHostSettings _hostSettings;

        public RabbitMqHostConfiguration(IRabbitMqBusConfiguration busConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostSettings = new ConfigurationHostSettings
            {
                Host = "localhost",
                VirtualHost = "/",
                Port = 5672,
                Username = "guest",
                Password = "guest"
            };

            var messageNameFormatter = new RabbitMqMessageNameFormatter();

            _topology = new RabbitMqBusTopology(this, messageNameFormatter, _hostSettings.HostAddress, topologyConfiguration);

            ReceiveTransportRetryPolicy = Retry.CreatePolicy(x =>
            {
                x.Handle<ConnectionException>();
                x.Handle<MessageNotConfirmedException>(exception =>
                    exception.Message.Contains("CONNECTION_FORCED")
                    || exception.Message.Contains("End of stream")
                    || exception.Message.Contains("Unexpected Exception"));

                x.Ignore<AuthenticationFailureException>();

                x.Exponential(1000, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3));
            });

            _connectionContext = new Recycle<IConnectionContextSupervisor>(() => new ConnectionContextSupervisor(this, topologyConfiguration));
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor => _connectionContext.Supervisor;

        public override Uri HostAddress => _hostSettings.HostAddress;

        public bool PublisherConfirmation => _hostSettings.PublisherConfirmation;

        public BatchSettings BatchSettings => _hostSettings.BatchSettings;

        IRabbitMqBusTopology IRabbitMqHostConfiguration.Topology => _topology;

        public override IRetryPolicy ReceiveTransportRetryPolicy { get; }

        public override IBusTopology Topology => _topology;

        public RabbitMqHostSettings Settings
        {
            get => _hostSettings;
            set => _hostSettings = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void ApplyEndpointDefinition(IRabbitMqReceiveEndpointConfigurator configurator, IEndpointDefinition definition)
        {
            if (definition.IsTemporary)
            {
                configurator.AutoDelete = true;
                configurator.Durable = false;
            }

            base.ApplyEndpointDefinition(configurator, definition);
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();
            var settings = new RabbitMqReceiveSettings(endpointConfiguration, queueName,
                _busConfiguration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, true, false);

            return CreateReceiveEndpointConfiguration(settings, endpointConfiguration, configure);
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(RabbitMqReceiveSettings settings,
            IRabbitMqEndpointConfiguration endpointConfiguration, Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (endpointConfiguration == null)
                throw new ArgumentNullException(nameof(endpointConfiguration));

            var configuration = new RabbitMqReceiveEndpointConfiguration(this, settings, endpointConfiguration);

            configure?.Invoke(configuration);

            Observers.EndpointConfigured(configuration);

            Add(configuration);

            return configuration;
        }

        public override void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, configurator =>
            {
                ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public override void ReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
        {
            CreateReceiveEndpointConfiguration(queueName, configureEndpoint);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (_hostSettings.BatchSettings.Enabled)
            {
                if (_hostSettings.BatchSettings.Timeout < TimeSpan.Zero || _hostSettings.BatchSettings.Timeout > TimeSpan.FromSeconds(1))
                    yield return this.Failure("BatchTimeout", "must be >= 0 and <= 1s");

                if (_hostSettings.BatchSettings.MessageLimit <= 1 || _hostSettings.BatchSettings.MessageLimit > 100)
                    yield return this.Failure("BatchMessageLimit", "must be >= 1 and <= 100");

                if (_hostSettings.BatchSettings.SizeLimit < 1024 || _hostSettings.BatchSettings.MessageLimit > 256 * 1024)
                    yield return this.Failure("BatchSizeLimit", "must be >= 1K and <= 256K");
            }
        }

        public override IReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IReceiveEndpointConfigurator> configure = null)
        {
            return CreateReceiveEndpointConfiguration(queueName, configure);
        }

        public override IHost Build()
        {
            var host = new RabbitMqHost(this, _topology);

            foreach (var endpointConfiguration in GetConfiguredEndpoints())
                endpointConfiguration.Build(host);

            return host;
        }
    }
}
