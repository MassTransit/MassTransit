namespace MassTransit
{
    using System;
    using AmazonSqsTransport;
    using AmazonSqsTransport.Configuration;
    using Configuration;
    using Topology;


    public static class AmazonSqsBusFactory
    {
        /// <summary>
        /// Configure and create a bus for AmazonSQS
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IAmazonSqsBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new AmazonSqsTopologyConfiguration(CreateMessageTopology());
            var busConfiguration = new AmazonSqsBusConfiguration(topologyConfiguration);

            var configurator = new AmazonSqsBusFactoryConfigurator(busConfiguration);

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
                EntityNameFormatter = new MessageNameFormatterEntityNameFormatter(new AmazonSqsMessageNameFormatter());
            }
        }
    }
}
