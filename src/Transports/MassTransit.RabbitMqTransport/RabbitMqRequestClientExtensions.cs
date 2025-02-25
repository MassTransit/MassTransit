﻿namespace MassTransit
{
    using RabbitMqTransport;


    public static class RabbitMqRequestClientExtensions
    {
        /// <summary>
        /// Creates a new RPC client factory on RabbitMQ using the direct reply-to feature
        /// </summary>
        /// <param name="connector">The connector, typically the bus instance</param>
        /// <param name="timeout">The default request timeout</param>
        /// <returns></returns>
        public static IClientFactory CreateReplyToClientFactory(this IReceiveConnector connector, RequestTimeout timeout = default)
        {
            var endpointDefinition = new ReplyToEndpointDefinition(default, 1000);

            var receiveEndpointHandle = connector.ConnectReceiveEndpoint(endpointDefinition, KebabCaseEndpointNameFormatter.Instance);

            return receiveEndpointHandle.CreateClientFactory(timeout);
        }

        /// <summary>
        /// Enables the RabbitMQ Reply-To response endpoint, which uses the AMQP reply-to header for replies without using the bus endpoint
        /// </summary>
        /// <param name="configurator"></param>
        public static void SetRabbitMqReplyToRequestClientFactory(this IBusRegistrationConfigurator configurator)
        {
            configurator.SetRequestClientFactory((bus, timeout) => CreateReplyToClientFactory(bus, timeout));
        }


        class ReplyToEndpointDefinition :
            IEndpointDefinition
        {
            public ReplyToEndpointDefinition(int? concurrentMessageLimit = default, int? prefetchCount = default)
            {
                ConcurrentMessageLimit = concurrentMessageLimit;
                PrefetchCount = prefetchCount;
            }

            public string GetEndpointName(IEndpointNameFormatter formatter)
            {
                return RabbitMqExchangeNames.ReplyTo;
            }

            public bool IsTemporary => false;
            public int? PrefetchCount { get; }
            public int? ConcurrentMessageLimit { get; }
            public bool ConfigureConsumeTopology => false;

            public void Configure<T>(T configurator, IRegistrationContext context)
                where T : IReceiveEndpointConfigurator
            {
            }
        }
    }
}
