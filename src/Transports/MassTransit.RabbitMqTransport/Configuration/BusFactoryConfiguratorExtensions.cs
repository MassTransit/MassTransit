namespace MassTransit
{
    using System;
    using RabbitMqTransport;
    using RabbitMqTransport.Configuration;


    public static class BusFactoryConfiguratorExtensions
    {
        /// <summary>
        /// Select RabbitMQ as the transport for the service bus
        /// </summary>
        public static IBusControl CreateUsingRabbitMq(this IBusFactorySelector selector, Action<IRabbitMqBusFactoryConfigurator> configure = null)
        {
            return RabbitMqBusFactory.Create(configure);
        }

        /// <summary>
        /// Configure MassTransit to use RabbitMQ for the transport.
        /// </summary>
        /// <param name="configurator">The registration configurator (configured via AddMassTransit)</param>
        /// <param name="configure">The configuration callback for the bus factory</param>
        /// <typeparam name="T"></typeparam>
        public static void UsingRabbitMq<T>(this IRegistrationConfigurator<T> configurator,
            Action<IRegistrationContext<T>, IRabbitMqBusFactoryConfigurator> configure = null)
            where T : class
        {
            configurator.SetBusFactory(new RabbitMqRegistrationBusFactory<T>(configure));
        }
    }
}
