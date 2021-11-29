namespace MassTransit
{
    using System;
    using RabbitMqTransport.Configuration;


    public static class RabbitMqBusFactoryConfiguratorExtensions
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
        public static void UsingRabbitMq(this IBusRegistrationConfigurator configurator,
            Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> configure = null)
        {
            configurator.SetBusFactory(new RabbitMqRegistrationBusFactory(configure));
        }
    }
}
