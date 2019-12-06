namespace MassTransit
{
    using System;
    using RabbitMqTransport;


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
        /// Add a RabbitMQ bus
        /// </summary>
        /// <param name="configurator">The registration configurator</param>
        /// <param name="configure">The configure callback method</param>
        /// <typeparam name="TContainerContext"></typeparam>
        public static void AddRabbitMqBus<TContainerContext>(this IRegistrationConfigurator<TContainerContext> configurator,
            Action<TContainerContext, IRabbitMqBusFactoryConfigurator> configure)
        {
            IBusControl BusFactory(TContainerContext context)
            {
                return RabbitMqBusFactory.Create(cfg =>
                {
                    configure(context, cfg);
                });
            }

            configurator.AddBus(BusFactory);
        }
    }
}
