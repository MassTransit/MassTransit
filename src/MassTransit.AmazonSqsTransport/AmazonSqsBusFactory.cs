namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using Configuration;
    using Configuration.Configuration;
    using Configuration.Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.EntityNameFormatters;
    using MassTransit.Topology.Topologies;


    public static class AmazonSqsBusFactory
    {
        public static IMessageTopologyConfigurator MessageTopology => Cached.MessageTopologyValue.Value;

        /// <summary>
        /// Configure and create a bus for AmazonSQS
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IAmazonSqsBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new AmazonSqsTopologyConfiguration(MessageTopology);
            var busConfiguration = new AmazonSqsBusConfiguration(topologyConfiguration);

            var configurator = new AmazonSqsBusFactoryConfigurator(busConfiguration);

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
                _entityNameFormatter = new MessageNameFormatterEntityNameFormatter(new AmazonSqsMessageNameFormatter());
            }
        }
    }
}
