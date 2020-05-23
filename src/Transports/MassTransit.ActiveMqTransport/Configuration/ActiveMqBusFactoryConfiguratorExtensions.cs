namespace MassTransit.ActiveMqTransport
{
    using System;
    using Configuration;


    public static class ActiveMqBusFactoryConfiguratorExtensions
    {
        /// <summary>
        /// Select ActiveMQ as the transport for the service bus
        /// </summary>
        public static IBusControl CreateUsingActiveMq(this IBusFactorySelector selector, Action<IActiveMqBusFactoryConfigurator> configure)
        {
            return ActiveMqBusFactory.Create(configure);
        }

        /// <summary>
        /// Configure MassTransit to use ActiveMQ for the transport.
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        /// <typeparam name="T"></typeparam>
        public static void UsingActiveMq<T>(this IRegistrationConfigurator<T> configurator,
            Action<IRegistrationContext<T>, IActiveMqBusFactoryConfigurator> configure = null)
            where T : class
        {
            configurator.SetBusFactory(new ActiveMqRegistrationBusFactory<T>(configure));
        }
    }
}
