namespace MassTransit
{
    using System;
    using Azure.ServiceBus.Core;
    using Turnout.Configuration;


    public static class ServiceBusTurnoutConfigurationExtensions
    {
        /// <summary>
        /// Configures a Turnout on the receive endpoint, which executes a long-running job and supervises the job until it
        /// completes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="busFactoryConfigurator">The bus factory configuration to use a separate endpoint for the control traffic</param>
        /// <param name="queueName"></param>
        /// <param name="configure"></param>
        /// <param name="host"></param>
        public static void TurnoutEndpoint<T>(this IServiceBusBusFactoryConfigurator busFactoryConfigurator, IServiceBusHost host, string queueName,
            Action<ITurnoutServiceConfigurator<T>> configure)
            where T : class
        {
            string expiredQueueName = $"{queueName}-expired";

            // configure the message expiration endpoint, so it's available at startup
            busFactoryConfigurator.ReceiveEndpoint(expiredQueueName, expiredEndpointConfigurator =>
            {
                expiredEndpointConfigurator.SubscribeMessageTopics = false;

                // configure the turnout management endpoint
                busFactoryConfigurator.ReceiveEndpoint(new TurnoutEndpointDefinition(),null, turnoutEndpointConfigurator =>
                {
                    turnoutEndpointConfigurator.PrefetchCount = 100;
                    turnoutEndpointConfigurator.SubscribeMessageTopics = false;

                    turnoutEndpointConfigurator.EnableDeadLetteringOnMessageExpiration = true;
                    turnoutEndpointConfigurator.ForwardDeadLetteredMessagesTo = expiredEndpointConfigurator.InputAddress.AbsolutePath.Trim('/');

                    turnoutEndpointConfigurator.AutoDeleteOnIdle = Defaults.TemporaryAutoDeleteOnIdle;
                    turnoutEndpointConfigurator.DefaultMessageTimeToLive = Defaults.TemporaryAutoDeleteOnIdle;

                    // configure the input queue endpoint
                    busFactoryConfigurator.ReceiveEndpoint(queueName, commandEndpointConfigurator =>
                    {
                        commandEndpointConfigurator.SubscribeMessageTopics = false;

                        commandEndpointConfigurator.ConfigureTurnoutEndpoints(busFactoryConfigurator, turnoutEndpointConfigurator, expiredEndpointConfigurator,
                            configure);
                    });
                });
            });
        }
    }
}
