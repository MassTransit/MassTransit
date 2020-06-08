namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Registration;


    public class RabbitMqRegistrationBusFactory :
        TransportRegistrationBusFactory
    {
        readonly RabbitMqBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext, IRabbitMqBusFactoryConfigurator> _configure;

        public RabbitMqRegistrationBusFactory(Action<IRegistrationContext, IRabbitMqBusFactoryConfigurator> configure)
            : this(new RabbitMqBusConfiguration(new RabbitMqTopologyConfiguration(RabbitMqBusFactory.MessageTopology)), configure)
        {
        }

        RabbitMqRegistrationBusFactory(RabbitMqBusConfiguration busConfiguration,
            Action<IRegistrationContext, IRabbitMqBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications)
        {
            var configurator = new RabbitMqBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
