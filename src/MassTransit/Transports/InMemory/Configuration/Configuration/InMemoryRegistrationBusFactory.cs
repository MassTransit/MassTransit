namespace MassTransit.Transports.InMemory.Configuration
{
    using System;
    using Configurators;
    using Registration;


    public class InMemoryRegistrationBusFactory<TContainerContext> :
        TransportRegistrationBusFactory<TContainerContext>
        where TContainerContext : class
    {
        readonly InMemoryBusConfiguration _busConfiguration;
        readonly Action<IRegistrationContext<TContainerContext>, IInMemoryBusFactoryConfigurator> _configure;

        public InMemoryRegistrationBusFactory(Uri baseAddress, Action<IRegistrationContext<TContainerContext>, IInMemoryBusFactoryConfigurator> configure)
            : this(new InMemoryBusConfiguration(new InMemoryTopologyConfiguration(InMemoryBus.MessageTopology), baseAddress), configure)
        {
        }

        InMemoryRegistrationBusFactory(InMemoryBusConfiguration busConfiguration,
            Action<IRegistrationContext<TContainerContext>, IInMemoryBusFactoryConfigurator> configure)
            : base(busConfiguration.HostConfiguration)
        {
            _configure = configure;

            _busConfiguration = busConfiguration;
        }

        public override IBusInstance CreateBus(IRegistrationContext<TContainerContext> context)
        {
            var configurator = new InMemoryBusFactoryConfigurator(_busConfiguration);

            return CreateBus(configurator, context, _configure);
        }
    }
}
