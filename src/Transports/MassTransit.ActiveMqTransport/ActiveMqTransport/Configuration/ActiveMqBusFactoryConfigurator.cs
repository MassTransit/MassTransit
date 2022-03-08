#nullable enable
namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Topology;


    public class ActiveMqBusFactoryConfigurator :
        BusFactoryConfigurator,
        IActiveMqBusFactoryConfigurator,
        IBusFactory
    {
        readonly IActiveMqBusConfiguration _busConfiguration;
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly ActiveMqQueueReceiveSettings _settings;

        public ActiveMqBusFactoryConfigurator(IActiveMqBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostConfiguration = busConfiguration.HostConfiguration;

            var queueName = _busConfiguration.Topology.Consume.CreateTemporaryQueueName("bus");

            _settings = new ActiveMqQueueReceiveSettings(busConfiguration.BusEndpointConfiguration, queueName, false, true);
        }

        public bool Durable
        {
            set => _settings.Durable = value;
        }

        public bool AutoDelete
        {
            set => _settings.AutoDelete = value;
        }

        public void Host(ActiveMqHostSettings settings)
        {
            _busConfiguration.HostConfiguration.Settings = settings;
        }

        void IActiveMqBusFactoryConfigurator.Send<T>(Action<IActiveMqMessageSendTopologyConfigurator<T>> configureTopology)
        {
            IActiveMqMessageSendTopologyConfigurator<T> configurator = _busConfiguration.Topology.Send.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        void IActiveMqBusFactoryConfigurator.Publish<T>(Action<IActiveMqMessagePublishTopologyConfigurator<T>> configureTopology)
        {
            IActiveMqMessagePublishTopologyConfigurator<T> configurator = _busConfiguration.Topology.Publish.GetMessageTopology<T>();

            configureTopology?.Invoke(configurator);
        }

        public new IActiveMqSendTopologyConfigurator SendTopology => _busConfiguration.Topology.Send;
        public new IActiveMqPublishTopologyConfigurator PublishTopology => _busConfiguration.Topology.Publish;

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IActiveMqReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _hostConfiguration.ReceiveEndpoint(queueName, configureEndpoint);
        }

        public void SetConsumerEndpointQueueNameFormatter(IActiveMqConsumerEndpointQueueNameFormatter formatter)
        {
            _busConfiguration.Topology.Consume.ConsumerEndpointQueueNameFormatter = formatter;
        }

        public void EnableArtemisCompatibility()
        {
            SetConsumerEndpointQueueNameFormatter(new ArtemisConsumerEndpointQueueNameFormatter());

            _hostConfiguration.IsArtemis = true;
        }

        public void SetTemporaryQueueNameFormatter(IActiveMqTemporaryQueueNameFormatter? formatter)
        {
            _busConfiguration.Topology.Consume.TemporaryQueueNameFormatter = formatter;
        }

        public void SetTemporaryQueueNamePrefix(string prefix)
        {
            SetTemporaryQueueNameFormatter(string.IsNullOrWhiteSpace(prefix) ? null : new PrefixTemporaryQueueNameFormatter(prefix));
        }

        public IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
        {
            var queueName = _busConfiguration.Topology.Consume.CreateTemporaryQueueName("bus");
            var settings = new ActiveMqQueueReceiveSettings(_busConfiguration.BusEndpointConfiguration, queueName, _settings.Durable, _settings.AutoDelete);

            return _busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration(settings, _busConfiguration.BusEndpointConfiguration, configure);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (string.IsNullOrWhiteSpace(_settings.EntityName))
                yield return this.Failure("Bus", "The bus queue name must not be null or empty");
        }
    }
}
