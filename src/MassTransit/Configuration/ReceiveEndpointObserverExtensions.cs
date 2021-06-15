namespace MassTransit
{
    using Configurators;
    using GreenPipes;
    using Registration;


    public static class ReceiveEndpointObserverExtensions
    {
        /// <summary>
        /// Connects a receive endpoint observer to the bus, that will be resolved from the container
        /// </summary>
        /// <param name="configurator">The bus configurator</param>
        /// <param name="provider">Typically use <see cref="IBusRegistrationContext"/> here when configuring the bus</param>
        /// <typeparam name="T">The observer type</typeparam>
        /// <returns>The observer connect handle (likely ignored)</returns>
        public static ConnectHandle ConnectReceiveEndpointObserver<T>(this IBusFactoryConfigurator configurator, IConfigurationServiceProvider provider)
            where T : class, IReceiveEndpointObserver
        {
            var observer = new ConnectReceiveEndpointObserverBusObserver<T>(provider);

            return configurator.ConnectBusObserver(observer);
        }
    }
}
