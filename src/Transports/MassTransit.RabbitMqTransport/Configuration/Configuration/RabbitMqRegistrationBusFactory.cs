namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using Configurators;
    using Registration;


    public class RabbitMqRegistrationBusFactory<TContainerContext> :
        TransportRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly RabbitMqBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext<TContainerContext>, IRabbitMqBusFactoryConfigurator> _configure;

        public RabbitMqRegistrationBusFactory(Action<IRegistrationContext<TContainerContext>, IRabbitMqBusFactoryConfigurator> configure)
            : this(new RabbitMqBusConfiguration(new RabbitMqTopologyConfiguration(RabbitMqBusFactory.MessageTopology)), configure)
        {
        }

        RabbitMqRegistrationBusFactory(RabbitMqBusConfiguration busConfiguration,
            Action<IRegistrationContext<TContainerContext>, IRabbitMqBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IRegistrationContext<TContainerContext> context)
        {
            var configurator = new RabbitMqBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure);
        }
    }
}
