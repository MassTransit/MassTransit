namespace MassTransit
{
    using System;
    using System.Threading;
    using ActiveMqTransport;
    using ActiveMqTransport.Configuration;
    using Configuration;
    using Topology;


    public static class ActiveMqBusFactory
    {
        public static IMessageTopologyConfigurator MessageTopology => Cached.MessageTopologyValue.Value;

        /// <summary>
        /// Configure and create a bus for ActiveMQ
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IActiveMqBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new ActiveMqTopologyConfiguration(MessageTopology);
            var busConfiguration = new ActiveMqBusConfiguration(topologyConfiguration);

            var configurator = new ActiveMqBusFactoryConfigurator(busConfiguration);

            configure(configurator);

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
                _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(new ActiveMqMessageNameFormatter());
            }
        }
    }
}
