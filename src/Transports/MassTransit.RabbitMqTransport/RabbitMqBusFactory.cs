namespace MassTransit
{
    using System;
    using System.Threading;
    using Configuration;
    using RabbitMqTransport;
    using RabbitMqTransport.Configuration;
    using Topology;


    public static class RabbitMqBusFactory
    {
        public static IMessageTopologyConfigurator MessageTopology => Cached.MessageTopologyValue.Value;

        /// <summary>
        /// Configure and create a bus for RabbitMQ
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IRabbitMqBusFactoryConfigurator> configure = null)
        {
            var topologyConfiguration = new RabbitMqTopologyConfiguration(MessageTopology);
            var busConfiguration = new RabbitMqBusConfiguration(topologyConfiguration);

            var configurator = new RabbitMqBusFactoryConfigurator(busConfiguration);

            configure?.Invoke(configurator);

            return configurator.Build(busConfiguration);
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
