namespace MassTransit.RabbitMqTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using BusConfigurators;
    using Configuration;
    using GreenPipes;
    using MassTransit.Builders;
    using Topology;
    using Topology.Settings;


    public class RabbitMqBusFactoryConfigurator :
        BusFactoryConfigurator,
        IRabbitMqBusFactoryConfigurator,
        IBusFactory
    {
        readonly IRabbitMqBusConfiguration _busConfiguration;
        readonly IRabbitMqHostConfiguration _hostConfiguration;
        readonly RabbitMqReceiveSettings _settings;

        public RabbitMqBusFactoryConfigurator(IRabbitMqBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostConfiguration = busConfiguration.HostConfiguration;

            var queueName = busConfiguration.Topology.Consume.CreateTemporaryQueueName("bus");
            var exchangeType = busConfiguration.BusEndpointConfiguration.Topology.Consume.ExchangeTypeSelector.DefaultExchangeType;
            _settings = new RabbitMqReceiveSettings(queueName, exchangeType, false, true);

            _settings.AutoDeleteAfter(TimeSpan.FromMinutes(1));
        }

        public IBusControl CreateBus()
        {
            void ConfigureBusEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
            {
                configurator.BindMessageExchanges = false;
            }

            var busReceiveEndpointConfiguration = _busConfiguration.HostConfiguration
                .CreateReceiveEndpointConfiguration(_settings, _busConfiguration.BusEndpointConfiguration, ConfigureBusEndpoint);

            var builder = new ConfigurationBusBuilder(_busConfiguration, busReceiveEndpointConfiguration);

            ApplySpecifications(builder);

            return builder.Build();
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (string.IsNullOrWhiteSpace(_settings.QueueName))
                yield return this.Failure("Bus", "The bus queue name must not be null or empty");
        }

        public ushort PrefetchCount
        {
            set => _settings.PrefetchCount = value;
        }

        public bool Durable
        {
            set => _settings.Durable = value;
        }

        public bool Exclusive
        {
            set => _settings.Exclusive = value;
        }

        public bool AutoDelete
        {
            set => _settings.AutoDelete = value;
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

        public bool Lazy
        {
            set => _settings.Lazy = value;
        }

        public TimeSpan? QueueExpiration
        {
            set => _settings.QueueExpiration = value;
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

        public RabbitMqEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return _settings.GetEndpointAddress(hostAddress);
        }

        public void EnablePriority(byte maxPriority)
        {
            _settings.EnablePriority(maxPriority);
        }

        public IRabbitMqHost Host(RabbitMqHostSettings settings)
        {
            _busConfiguration.HostConfiguration.Settings = settings;

            return _busConfiguration.HostConfiguration.Proxy;
        }

        void IRabbitMqBusFactoryConfigurator.Send<T>(Action<IRabbitMqMessageSendTopologyConfigurator<T>> configureTopology)
        {
            IRabbitMqMessageSendTopologyConfigurator<T> configurator = _busConfiguration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        void IRabbitMqBusFactoryConfigurator.Publish<T>(Action<IRabbitMqMessagePublishTopologyConfigurator<T>> configureTopology)
        {
            IRabbitMqMessagePublishTopologyConfigurator<T> configurator = _busConfiguration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IRabbitMqSendTopologyConfigurator SendTopology => _busConfiguration.Topology.Send;
        public new IRabbitMqPublishTopologyConfigurator PublishTopology => _busConfiguration.Topology.Publish;

        public bool DeployTopologyOnly
        {
            set => _hostConfiguration.DeployTopologyOnly = value;
        }

        public void OverrideDefaultBusEndpointQueueName(string queueName)
        {
            _settings.ExchangeName = queueName;
            _settings.QueueName = queueName;
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IRabbitMqHost host, IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(IRabbitMqHost host, string queueName, Action<IRabbitMqReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }
    }
}
