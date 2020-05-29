namespace MassTransit
{
    using System;


    public static class BusObserverExtensions
    {
        /// <summary>
        /// Connect an observer to the bus, to observe creation, start, stop, fault events.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="observer"></param>
        public static void BusObserver(this IBusFactoryConfigurator configurator, IBusObserver observer)
        {
            configurator.ConnectBusObserver(observer);
        }

        /// <summary>
        /// Connect an observer to the bus, to observe creation, start, stop, fault events.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="observerFactory">Factory to create the bus observer</param>
        public static void BusObserver<T>(this IBusFactoryConfigurator configurator, Func<T> observerFactory)
            where T : IBusObserver
        {
            configurator.ConnectBusObserver(observerFactory());
        }
    }
}
