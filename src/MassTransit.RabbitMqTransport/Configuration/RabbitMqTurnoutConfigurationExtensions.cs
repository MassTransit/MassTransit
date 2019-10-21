namespace MassTransit
{
    using System;
    using RabbitMqTransport;
    using Turnout.Configuration;


    public static class RabbitMqTurnoutConfigurationExtensions
    {
        /// <summary>
        /// Creates a Turnout endpoint on the bus, which is capable of executing long-running jobs without hanging the consumer pipeline.
        /// Multiple receive endpoints are created, including the main queue, an expired queue, and a management queue for communicating
        /// back to the turnout coordinator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busFactoryConfigurator">The bus factory configuration to use a separate endpoint for the control traffic</param>
        /// <param name="queueName">The receive queue name for commands</param>
        /// <param name="configure"></param>
        /// <param name="host">The host on which to configure the endpoint</param>
        public static void TurnoutEndpoint<T>(this IRabbitMqBusFactoryConfigurator busFactoryConfigurator, IRabbitMqHost host, string queueName,
            Action<ITurnoutServiceConfigurator<T>> configure)
            where T : class
        {
            string expiredQueueName = $"{queueName}-expired";

            // configure the message expiration endpoint, so it's available at startup
            busFactoryConfigurator.ReceiveEndpoint(expiredQueueName, expiredEndpointConfigurator =>
            {
                expiredEndpointConfigurator.BindMessageExchanges = false;

                // configure the turnout management endpoint
                busFactoryConfigurator.ReceiveEndpoint(new TurnoutEndpointDefinition(), null, turnoutEndpointConfigurator =>
                {
                    turnoutEndpointConfigurator.PrefetchCount = 100;
                    turnoutEndpointConfigurator.AutoDelete = true;
                    turnoutEndpointConfigurator.Durable = false;

                    turnoutEndpointConfigurator.DeadLetterExchange = expiredQueueName;

                    // configure the input queue endpoint
                    busFactoryConfigurator.ReceiveEndpoint(queueName, commandEndpointConfigurator =>
                    {
                        commandEndpointConfigurator.ConfigureTurnoutEndpoints(busFactoryConfigurator, turnoutEndpointConfigurator, expiredEndpointConfigurator,
                            configure);
                    });
                });
            });
        }
    }
}
