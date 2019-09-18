namespace MassTransit
{
    using System;
    using System.Threading;
    using Topology;
    using Topology.EntityNameFormatters;
    using Topology.Topologies;
    using Transports.InMemory.Configuration;
    using Transports.InMemory.Configurators;


    public static class InMemoryBus
    {
        public static IMessageTopologyConfigurator MessageTopology => Cached.MessageTopologyValue.Value;

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
            var topologyConfiguration = new InMemoryTopologyConfiguration(MessageTopology);
            var busConfiguration = new InMemoryBusConfiguration(topologyConfiguration, baseAddress);

            var configurator = new InMemoryBusFactoryConfigurator(busConfiguration);

            configure(configurator);

            return configurator.Build();
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageTopologyConfigurator> MessageTopologyValue =
                new Lazy<IMessageTopologyConfigurator>(() => new MessageTopology(_entityNameFormatter), LazyThreadSafetyMode.PublicationOnly);

            static readonly IEntityNameFormatter _entityNameFormatter;

            static Cached()
            {
                _entityNameFormatter = new MessageUrnEntityNameFormatter();
            }
        }
    }
}
