namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Clients;
    using Clients.Contexts;
    using Definition;
    using RabbitMqTransport;


    public static class RabbitMqRequestClientExtensions
    {
        /// <summary>
        /// Creates a new RPC client factory on RabbitMQ using the direct reply-to feature
        /// </summary>
        /// <param name="connector">The connector, typically the bus instance</param>
        /// <param name="timeout">The default request timeout</param>
        /// <returns></returns>
        public static Task<IClientFactory> CreateReplyToClientFactory(this IReceiveConnector connector, RequestTimeout timeout = default)
        {
            var endpointDefinition = new ReplyToEndpointDefinition(1000);

            var receiveEndpointHandle = connector.ConnectReceiveEndpoint(endpointDefinition, KebabCaseEndpointNameFormatter.Instance);

            return receiveEndpointHandle.CreateClientFactory(timeout);
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

            public void Configure<T>(T configurator)
                where T : IReceiveEndpointConfigurator
            {
            }
        }


        /// <summary>
        /// Creates a request client that uses the bus to retrieve the endpoint and send the request.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="host"></param>
        /// <param name="destinationAddress">The service address that handles the request</param>
        /// <param name="timeout">The timeout before the request is cancelled</param>
        /// <param name="timeToLive">THe time to live for the request message</param>
        /// <param name="callback">Callback when the request is sent</param>
        /// <returns></returns>
        public static async Task<IRequestClient<TRequest, TResponse>> CreateRequestClient<TRequest, TResponse>(this IRabbitMqHost host, Uri destinationAddress,
            TimeSpan timeout, TimeSpan? timeToLive = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var clientFactory = await host.CreateClientFactory(timeout).ConfigureAwait(false);

            IRequestClient<TRequest> requestClient = clientFactory.CreateRequestClient<TRequest>(destinationAddress);

            return new MessageRequestClient<TRequest, TResponse>(requestClient, timeToLive, callback);
        }

        /// <summary>
        /// Creates a request client that uses the bus to publish a request.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="timeout">The timeout before the request is cancelled</param>
        /// <param name="callback">Callback when the request is sent</param>
        /// <param name="timeToLive">The time that the request will live for</param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static async Task<IRequestClient<TRequest, TResponse>> CreatePublishRequestClient<TRequest, TResponse>(this IRabbitMqHost host, TimeSpan timeout,
            TimeSpan? timeToLive = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var clientFactory = await host.CreateClientFactory(timeout).ConfigureAwait(false);

            IRequestClient<TRequest> requestClient = clientFactory.CreateRequestClient<TRequest>();

            return new MessageRequestClient<TRequest, TResponse>(requestClient, timeToLive, callback);
        }

        /// <summary>
        /// Creates a request client factory which can be used to create a request client per message within a consume context.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">The host for the response endpoint</param>
        /// <param name="destinationAddress">The service address</param>
        /// <param name="timeout">The request timeout</param>
        /// <param name="timeToLive">The request time to live</param>
        /// <param name="callback">Customize the send context</param>
        /// <returns></returns>
        public static async Task<IRequestClientFactory<TRequest, TResponse>> CreateRequestClientFactory<TRequest, TResponse>(this IRabbitMqHost host,
            Uri destinationAddress, TimeSpan timeout, TimeSpan? timeToLive = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var receiveEndpointHandle = host.ConnectResponseEndpoint();

            var ready = await receiveEndpointHandle.Ready.ConfigureAwait(false);

            var context = new HostReceiveEndpointClientFactoryContext(receiveEndpointHandle, ready, timeout);

            IClientFactory clientFactory = new ClientFactory(context);

            return new MessageRequestClientFactory<TRequest, TResponse>(context, clientFactory, destinationAddress, timeToLive, callback);
        }

        /// <summary>
        /// Creates a request client factory which can be used to create a request client per message within a consume context.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">The host for the response endpoint</param>
        /// <param name="timeout">The request timeout</param>
        /// <param name="timeToLive">The request time to live</param>
        /// <param name="callback">Customize the send context</param>
        /// <returns></returns>
        public static async Task<IRequestClientFactory<TRequest, TResponse>> CreatePublishRequestClientFactory<TRequest, TResponse>(this IRabbitMqHost host,
            TimeSpan timeout, TimeSpan? timeToLive = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var receiveEndpointHandle = host.ConnectResponseEndpoint();

            var ready = await receiveEndpointHandle.Ready.ConfigureAwait(false);

            var context = new HostReceiveEndpointClientFactoryContext(receiveEndpointHandle, ready, timeout);

            IClientFactory clientFactory = new ClientFactory(context);

            return new MessageRequestClientFactory<TRequest, TResponse>(context, clientFactory, null, timeToLive, callback);
        }
    }
}
