namespace MassTransit
{
    using System;
    using Configuration;
    using InMemoryTransport.Configuration;
    using Topology;


    public static class InMemoryBus
    {
        /// <summary>
        /// Configure and create an in-memory bus
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IInMemoryBusFactoryConfigurator> configure)
        {
            return Create(null, configure);
        }

        /// <summary>
        /// Configure and create an in-memory bus
        /// </summary>
        /// <param name="baseAddress">Override the default base address</param>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Uri baseAddress, Action<IInMemoryBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new InMemoryTopologyConfiguration(CreateMessageTopology());
            var busConfiguration = new InMemoryBusConfiguration(topologyConfiguration, baseAddress);

            var configurator = new InMemoryBusFactoryConfigurator(busConfiguration);

            configure(configurator);

            return configurator.Build(busConfiguration);
        }

        public static IMessageTopologyConfigurator CreateMessageTopology()
        {
            return new MessageTopology(Cached.EntityNameFormatter);
        }


        static class Cached
        {
            internal static readonly IEntityNameFormatter EntityNameFormatter;

            static Cached()
            {
                EntityNameFormatter = new MessageUrnEntityNameFormatter();
            }
        }
    }
}
