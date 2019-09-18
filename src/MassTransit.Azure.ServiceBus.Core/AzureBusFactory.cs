namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Threading;
    using Configuration;
    using Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.EntityNameFormatters;
    using MassTransit.Topology.Topologies;


    public static class AzureBusFactory
    {
        public static IMessageTopologyConfigurator MessageTopology => Cached.MessageTopologyValue.Value;

        /// <summary>
        /// Configure and create a bus for Azure Service Bus (later, we'll use Event Hubs instead)
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl CreateUsingServiceBus(Action<IServiceBusBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new ServiceBusTopologyConfiguration(MessageTopology);
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);

            var configurator = new ServiceBusBusFactoryConfigurator(busConfiguration);

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
                _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(new ServiceBusMessageNameFormatter());
            }
        }
    }
}
