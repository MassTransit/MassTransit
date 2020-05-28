namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Registration;


    public class ActiveMqRegistrationBusFactory<TContainerContext> :
        TransportRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly ActiveMqBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext<TContainerContext>, IActiveMqBusFactoryConfigurator> _configure;

        public ActiveMqRegistrationBusFactory(Action<IRegistrationContext<TContainerContext>, IActiveMqBusFactoryConfigurator> configure)
            : this(new ActiveMqBusConfiguration(new ActiveMqTopologyConfiguration(ActiveMqBusFactory.MessageTopology)), configure)
        {
        }

        ActiveMqRegistrationBusFactory(ActiveMqBusConfiguration busConfiguration,
            Action<IRegistrationContext<TContainerContext>, IActiveMqBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IRegistrationContext<TContainerContext> context, IEnumerable<IBusInstanceSpecification> specifications)
        {
            var configurator = new ActiveMqBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
