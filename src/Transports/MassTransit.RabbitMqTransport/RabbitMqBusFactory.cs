namespace MassTransit
{
    using System;
    using Configuration;
    using RabbitMqTransport;
    using RabbitMqTransport.Configuration;
    using Topology;


    public static class RabbitMqBusFactory
    {
        /// <summary>
        /// Configure and create a bus for RabbitMQ
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IRabbitMqBusFactoryConfigurator> configure = null)
        {
            var topologyConfiguration = new RabbitMqTopologyConfiguration(CreateMessageTopology());
            var busConfiguration = new RabbitMqBusConfiguration(topologyConfiguration);

            var configurator = new RabbitMqBusFactoryConfigurator(busConfiguration);

            configure?.Invoke(configurator);

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
                EntityNameFormatter = new MessageNameFormatterEntityNameFormatter(new RabbitMqMessageNameFormatter());
            }
        }
    }
}
