namespace MassTransit
{
    using System;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;
    using Configuration;
    using Topology;


    public static class AzureBusFactory
    {
        /// <summary>
        /// Configure and create a bus for Azure Service Bus (later, we'll use Event Hubs instead)
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl CreateUsingServiceBus(Action<IServiceBusBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new ServiceBusTopologyConfiguration(CreateMessageTopology());
            var busConfiguration = new ServiceBusBusConfiguration(topologyConfiguration);

            var configurator = new ServiceBusBusFactoryConfigurator(busConfiguration);

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
                EntityNameFormatter = new MessageNameFormatterEntityNameFormatter(new ServiceBusMessageNameFormatter());
            }
        }
    }
}
