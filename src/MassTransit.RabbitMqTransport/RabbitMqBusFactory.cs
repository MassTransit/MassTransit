namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading;
    using Configuration;
    using Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.EntityNameFormatters;
    using MassTransit.Topology.Topologies;


    public static class RabbitMqBusFactory
    {
        public static IMessageTopologyConfigurator MessageTopology => Cached.MessageTopologyValue.Value;

        /// <summary>
        /// Configure and create a bus for RabbitMQ
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IRabbitMqBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new RabbitMqTopologyConfiguration(MessageTopology);
            var busConfiguration = new RabbitMqBusConfiguration(topologyConfiguration);

            var configurator = new RabbitMqBusFactoryConfigurator(busConfiguration);

            configure(configurator);

            return configurator.Build();
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageTopologyConfigurator> MessageTopologyValue =
                new Lazy<IMessageTopologyConfigurator>(() => new MessageTopology(_entityNameFormatter),
                    LazyThreadSafetyMode.PublicationOnly);

            static readonly IEntityNameFormatter _entityNameFormatter;

            static Cached()
            {
                _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(new RabbitMqMessageNameFormatter());
            }
        }
    }
}
