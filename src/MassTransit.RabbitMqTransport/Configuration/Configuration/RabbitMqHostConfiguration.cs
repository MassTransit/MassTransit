namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Definition;
    using GreenPipes;
    using MassTransit.Configurators;
    using Topology;
    using Topology.Settings;
    using Topology.Topologies;
    using Transport;
    using Transports;


    public class RabbitMqHostConfiguration :
        BaseHostConfiguration<IRabbitMqReceiveEndpointConfiguration>,
        IRabbitMqHostConfiguration
    {
        readonly IRabbitMqBusConfiguration _busConfiguration;
        readonly RabbitMqHostProxy _proxy;
        readonly IRabbitMqTopologyConfiguration _topologyConfiguration;
        RabbitMqHostSettings _hostSettings;

        public RabbitMqHostConfiguration(IRabbitMqBusConfiguration busConfiguration, IRabbitMqTopologyConfiguration topologyConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _topologyConfiguration = topologyConfiguration;
            _hostSettings = new ConfigurationHostSettings
            {
                Host = "localhost",
                VirtualHost = "/",
                Port = 5672,
                Username = "guest",
                Password = "guest"
            };

            _proxy = new RabbitMqHostProxy(this);
        }

        public string Description => _hostSettings.ToDescription();

        public override Uri HostAddress => _hostSettings.HostAddress;

        public IRabbitMqHost Proxy => _proxy;

        public bool PublisherConfirmation => _hostSettings.PublisherConfirmation;

        public BatchSettings BatchSettings => _hostSettings.BatchSettings;

        public bool DeployTopologyOnly { get; set; }

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

            if (definition.PrefetchCount.HasValue)
            {
                configurator.PrefetchCount = (ushort)definition.PrefetchCount.Value;
            }

            if (definition.ConcurrentMessageLimit.HasValue)
            {
                var concurrentMessageLimit = definition.ConcurrentMessageLimit.Value;

                // if there is a prefetchCount, and it is greater than the concurrent message limit, we need a filter
                if (!definition.PrefetchCount.HasValue || definition.PrefetchCount.Value > concurrentMessageLimit)
                {
                    configurator.UseConcurrencyLimit(concurrentMessageLimit);

                    // we should determine a good value to use based upon the concurrent message limit
                    if (definition.PrefetchCount.HasValue == false)
                    {
                        var calculatedPrefetchCount = concurrentMessageLimit * 12 / 10;

                        configurator.PrefetchCount = (ushort)calculatedPrefetchCount;
                    }
                }
            }

            definition.Configure(configurator);
        }

        public IRabbitMqReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IRabbitMqReceiveEndpointConfigurator> configure)
        {
            var settings = new RabbitMqReceiveSettings(queueName, _busConfiguration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType, true, false);
            var endpointConfiguration = _busConfiguration.CreateEndpointConfiguration();

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

        void IReceiveConfigurator.ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, configureEndpoint);
        }

        void IReceiveConfigurator.ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            ReceiveEndpoint(queueName, configurator =>
            {
                ApplyEndpointDefinition(configurator, definition);
                configureEndpoint?.Invoke(configurator);
            });
        }

        public void ReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
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

        public override IBusHostControl Build()
        {
            var exchangeTypeSelector = new FanoutExchangeTypeSelector();
            var messageNameFormatter = new RabbitMqMessageNameFormatter();

            var hostTopology = new RabbitMqHostTopology(exchangeTypeSelector, messageNameFormatter, _hostSettings.HostAddress, _topologyConfiguration);

            var host = new RabbitMqHost(this, hostTopology);

            foreach (var endpointConfiguration in Endpoints)
            {
                endpointConfiguration.Build(host);
            }

            _proxy.SetComplete(host);

            return host;
        }
    }
}
