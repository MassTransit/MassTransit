namespace MassTransit
{
    using System;
    using System.Threading;
    using Configuration;
    using SqlTransport.Configuration;
    using SqlTransport.Topology;
    using Topology;


    public static class SqlBusFactory
    {
        public static IMessageTopologyConfigurator MessageTopology => Cached.MessageTopologyValue.Value;

        /// <summary>
        /// Create a bus using the database transport
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<ISqlBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new SqlTopologyConfiguration(MessageTopology);
            var busConfiguration = new SqlBusConfiguration(topologyConfiguration);

            var configurator = new SqlBusFactoryConfigurator(busConfiguration);

            configure(configurator);

            return configurator.Build(busConfiguration);
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageTopologyConfigurator> MessageTopologyValue;

            static Cached()
            {
                IEntityNameFormatter formatter = new MessageNameFormatterEntityNameFormatter(new SqlMessageNameFormatter());
                MessageTopologyValue = new Lazy<IMessageTopologyConfigurator>(() => new MessageTopology(formatter), LazyThreadSafetyMode.PublicationOnly);
            }
        }
    }
}
