namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using System.Collections.Generic;
    using Configurators;
    using Registration;


    public class InMemoryRegistrationBusFactory :
        TransportRegistrationBusFactory
    {
        readonly InMemoryBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext, IInMemoryBusFactoryConfigurator> _configure;

        public InMemoryRegistrationBusFactory(Uri baseAddress, Action<IRegistrationContext, IInMemoryBusFactoryConfigurator> configure)
            : this(new InMemoryBusConfiguration(new InMemoryTopologyConfiguration(InMemoryBus.MessageTopology), baseAddress), configure)
        {
        }

        InMemoryRegistrationBusFactory(InMemoryBusConfiguration busConfiguration,
            Action<IRegistrationContext, IInMemoryBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications)
        {
            var configurator = new InMemoryBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure, specifications);
        }
    }
}
