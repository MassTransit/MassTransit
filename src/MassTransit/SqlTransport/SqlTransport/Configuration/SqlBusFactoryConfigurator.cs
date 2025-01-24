#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;


    public class SqlBusFactoryConfigurator :
        BusFactoryConfigurator,
        ISqlBusFactoryConfigurator,
        IBusFactory
    {
        readonly ISqlBusConfiguration _busConfiguration;
        readonly ISqlHostConfiguration _hostConfiguration;
        readonly SqlReceiveSettings _settings;

        public SqlBusFactoryConfigurator(ISqlBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostConfiguration = busConfiguration.HostConfiguration;

            var queueName = busConfiguration.Topology.Consume.CreateTemporaryQueueName("bus");
            _settings = new SqlReceiveSettings(busConfiguration.BusEndpointConfiguration, queueName, Defaults.TemporaryAutoDeleteOnIdle);
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

        public TimeSpan? AutoDeleteOnIdle
        {
            set => _settings.AutoDeleteOnIdle = value;
        }

        public TimeSpan PollingInterval
        {
            set => _settings.PollingInterval = value;
        }

        public TimeSpan LockDuration
        {
            set => _settings.LockDuration = value;
        }

        public TimeSpan MaxLockDuration
        {
            set => _settings.MaxLockDuration = value;
        }

        public int? MaxDeliveryCount
        {
            set => _settings.MaxDeliveryCount = value;
        }

        public bool PurgeOnStartup
        {
            set => _settings.PurgeOnStartup = value;
        }

        public int MaintenanceBatchSize
        {
            set => _settings.MaintenanceBatchSize = value;
        }

        public bool DeadLetterExpiredMessages
        {
            set => _settings.DeadLetterExpiredMessages = value;
        }

        public void Host(SqlHostSettings settings)
        {
            _busConfiguration.HostConfiguration.Settings = settings;
        }

        public void Send<T>(Action<ISqlMessageSendTopologyConfigurator<T>>? configureTopology)
            where T : class
        {
            ISqlMessageSendTopologyConfigurator<T> configurator = _busConfiguration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Publish<T>(Action<ISqlMessagePublishTopologyConfigurator<T>>? configureTopology)
            where T : class
        {
            ISqlMessagePublishTopologyConfigurator<T>? configurator = _busConfiguration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public void Publish(Type messageType, Action<ISqlMessagePublishTopologyConfigurator>? configure = null)
        {
            var configurator = _busConfiguration.Topology.Publish.GetMessageTopology(messageType);

            configure?.Invoke(configurator);
        }

        public new ISqlSendTopologyConfigurator SendTopology => _busConfiguration.Topology.Send;
        public new ISqlPublishTopologyConfigurator PublishTopology => _busConfiguration.Topology.Publish;

        public void OverrideDefaultBusEndpointQueueName(string queueName)
        {
            _settings.QueueName = queueName;
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<ISqlReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<ISqlReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }
    }
}
