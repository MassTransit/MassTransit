namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Registration;


    public class ActiveMqRegistrationBusFactory :
        TransportRegistrationBusFactory
    {
        readonly ActiveMqBusConfiguration _busConfiguration;
        readonly Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator> _configure;

        public ActiveMqRegistrationBusFactory(Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator> configure)
            : this(new ActiveMqBusConfiguration(new ActiveMqTopologyConfiguration(ActiveMqBusFactory.MessageTopology)), configure)
        {
        }

        ActiveMqRegistrationBusFactory(ActiveMqBusConfiguration busConfiguration,
            Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications)
        {
            var configurator = new ActiveMqBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
