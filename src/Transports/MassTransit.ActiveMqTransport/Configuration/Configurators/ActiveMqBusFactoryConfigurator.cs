namespace MassTransit.ActiveMqTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using BusConfigurators;
    using Configuration;
    using Configuration.Definition;
    using GreenPipes;
    using MassTransit.Configuration;
    using Topology;
    using Topology.Settings;


    public class ActiveMqBusFactoryConfigurator :
        BusFactoryConfigurator,
        IActiveMqBusFactoryConfigurator,
        IBusFactory
    {
        readonly IActiveMqBusConfiguration _busConfiguration;
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly QueueReceiveSettings _settings;

        public ActiveMqBusFactoryConfigurator(IActiveMqBusConfiguration busConfiguration)
            : base(busConfiguration)
        {
            _busConfiguration = busConfiguration;
            _hostConfiguration = busConfiguration.HostConfiguration;

            var queueName = _busConfiguration.Topology.Consume.CreateTemporaryQueueName("bus");

            _settings = new QueueReceiveSettings(busConfiguration.BusEndpointConfiguration, queueName, false, true);
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
        public new IActiveMqConsumeTopologyConfigurator ConsumeTopology => _busConfiguration.Topology.Consume;

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IActiveMqReceiveEndpointConfigurator> configureEndpoint = null)
        {
            _hostConfiguration.ReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
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

        public IReceiveEndpointConfiguration CreateBusEndpointConfiguration(Action<IReceiveEndpointConfigurator> configure)
        {
            var queueName = _busConfiguration.Topology.Consume.CreateTemporaryQueueName("bus");
            var settings = new QueueReceiveSettings(_busConfiguration.BusEndpointConfiguration, queueName, _settings.Durable, _settings.AutoDelete);

            return _busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration(settings, _busConfiguration.BusEndpointConfiguration, configure);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (string.IsNullOrWhiteSpace(_settings.EntityName))
                yield return this.Failure("Bus", "The bus queue name must not be null or empty");

        }

        public void EnableArtemisCompatibility()
        {
            ConsumeTopology.ConsumerEndpointQueueNameFormatter = new ArtemisConsumerEndpointQueueNameFormatter();
        }

        public void SetPrefixForTemporaryQueueNames(string prefix)
        {
            ConsumeTopology.TemporaryQueueNameFormatter = new PrefixTemporaryQueueNameFormatter(prefix);
        }
    }
}
