namespace MassTransit
{
    using System;
    using Configuration;
    using SqlTransport.Configuration;
    using SqlTransport.Topology;
    using Topology;


    public static class SqlBusFactory
    {
        /// <summary>
        /// Create a bus using the database transport
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<ISqlBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new SqlTopologyConfiguration(CreateMessageTopology());
            var busConfiguration = new SqlBusConfiguration(topologyConfiguration);

            var configurator = new SqlBusFactoryConfigurator(busConfiguration);

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
                EntityNameFormatter = new MessageNameFormatterEntityNameFormatter(new SqlMessageNameFormatter());
            }
        }
    }
}
