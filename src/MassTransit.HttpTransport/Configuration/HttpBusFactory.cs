namespace MassTransit.HttpTransport
{
    using System;
    using Configuration;
    using Specifications;


    public static class HttpBusFactory
    {
        /// <summary>
        /// Configure and create a bus for Http
        /// </summary>
        /// <param name="configure">The configuration callback to configure the bus</param>
        /// <returns></returns>
        public static IBusControl Create(Action<IHttpBusFactoryConfigurator> configure)
        {
            var topologyConfiguration = new HttpTopologyConfiguration(InMemoryBus.MessageTopology);
            var busConfiguration = new HttpBusConfiguration(topologyConfiguration);

            var configurator = new HttpBusFactoryConfigurator(busConfiguration);

            configure(configurator);

            return configurator.Build();
        }
    }
}
