#nullable enable
namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;


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
            _settings = new RabbitMqReceiveSettings(busConfiguration.BusEndpointConfiguration, queueName, exchangeType, false, true);

            _settings.AutoDeleteAfter(TimeSpan.FromMinutes(1));
        }

        public IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
        {
            return _busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration(_settings, _busConfiguration.BusEndpointConfiguration, configure);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (string.IsNullOrWhiteSpace(_settings.QueueName))
                yield return this.Failure("Bus", "The bus queue name must not be null or empty");
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

        public bool SingleActiveConsumer
        {
            set => _settings.SingleActiveConsumer = value;
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

        public void Host(RabbitMqHostSettings settings)
        {
            _busConfiguration.HostConfiguration.Settings = settings;
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

        public void OverrideDefaultBusEndpointQueueName(string queueName)
        {
            _settings.ExchangeName = queueName;
            _settings.QueueName = queueName;
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IRabbitMqReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint)
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
    }
}
