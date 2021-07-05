namespace MassTransit.ActiveMqTransport.Configurators
{
    using System;
    using System.Collections.Generic;
    using BusConfigurators;
    using Configuration;
    using GreenPipes;
    using MassTransit.ActiveMqTransport.Topology.Specifications;
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
            return _busConfiguration.HostConfiguration.CreateReceiveEndpointConfiguration(_settings, _busConfiguration.BusEndpointConfiguration, configure);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            foreach (var result in base.Validate())
                yield return result;

            if (string.IsNullOrWhiteSpace(_settings.EntityName))
                yield return this.Failure("Bus", "The bus queue name must not be null or empty");
        }

        /// <summary>
        /// New configurable extensionpoint, available/accessble via the configurator.
        /// This extension point can be used to enable artimis compatibility
        /// It allows to override the generated BindingConsumeTopologySpecification
        /// That specification is responsible (on calling its Apply method) for interacting with an instance of IReceiveEndpointBrokerTopologyBuilder.
        /// This allows the specification to control what queues, bindings, are created.
        /// For the interop with Artemis the name of the consumer queue is important
        /// The original specification was : ActiveMqBindConsumeTopologySpecification
        /// a new one has been already provided for Artemis => ArtemisBindConsumeTopologySpecification
        ///
        /// When you call EnableArtemisCompatibility() => this factory method is automatically initialized with a factory method that will
        /// create a ArtemisBindConsumeTopologySpecification
        ///
        /// This extension could also be used to create your own specifications if you like other behavior for creating queues and bindings or
        /// name conventions
        /// </summary>
        public ActiveMqBindingConsumeTopologySpecificationFactoryMethod BindingConsumeTopologySpecificationFactoryMethod
        {
            get => _busConfiguration.BindingConsumeTopologySpecificationFactoryMethod;
            set => _busConfiguration.BindingConsumeTopologySpecificationFactoryMethod = value;
        }

        /// <summary>
        /// Handy shortcut utility function for enabling Artemis compatibility.
        /// </summary>
        public void EnableArtemisCompatibility()
        {
            BindingConsumeTopologySpecificationFactoryMethod = (string topic) =>
            {
                return new ArtemisBindConsumeTopologySpecification(topic);
            };
        }

        public void UpdateReceiveQueueName(Func<string, string> transformReceiveQueueName)
        {
            var entityNameTransformed = transformReceiveQueueName?.Invoke(_settings.EntityName);
            _settings.EntityName = entityNameTransformed;
        }
    }
}
