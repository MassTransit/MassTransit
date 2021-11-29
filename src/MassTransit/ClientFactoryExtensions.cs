namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using Clients;


    public static class ClientFactoryExtensions
    {
        /// <summary>
        /// Create a request client from the bus, using the default bus endpoint for responses
        /// </summary>
        /// <param name="bus">The bus instance</param>
        /// <param name="destinationAddress">The request service address</param>
        /// <param name="timeout">The default request timeout</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        public static IRequestClient<TRequest> CreateRequestClient<TRequest>(this IBus bus, Uri destinationAddress, RequestTimeout timeout = default)
            where TRequest : class
        {
            var clientFactory = new ClientFactory(new BusClientFactoryContext(bus, timeout));

            return clientFactory.CreateRequestClient<TRequest>(destinationAddress, timeout);
        }

        /// <summary>
        /// Create a request client from the bus, using the default bus endpoint for responses, and publishing the request versus sending it.
        /// </summary>
        /// <param name="bus">The bus instance</param>
        /// <param name="timeout">The default request timeout</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        public static IRequestClient<TRequest> CreateRequestClient<TRequest>(this IBus bus, RequestTimeout timeout = default)
            where TRequest : class
        {
            var clientFactory = new ClientFactory(new BusClientFactoryContext(bus, timeout));

            return clientFactory.CreateRequestClient<TRequest>(timeout);
        }

        /// <summary>
        /// Create a request client from the bus, using the default bus endpoint for responses
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="bus">The bus instance</param>
        /// <param name="destinationAddress">The request service address</param>
        /// <param name="timeout">The default request timeout</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        public static IRequestClient<TRequest> CreateRequestClient<TRequest>(this ConsumeContext consumeContext, IBus bus, Uri destinationAddress,
            RequestTimeout timeout = default)
            where TRequest : class
        {
            var clientFactory = new ClientFactory(new BusClientFactoryContext(bus, timeout));

            return clientFactory.CreateRequestClient<TRequest>(consumeContext, destinationAddress, timeout);
        }

        /// <summary>
        /// Create a request client from the bus, using the default bus endpoint for responses
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="bus">The bus instance</param>
        /// <param name="timeout">The default request timeout</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        public static IRequestClient<TRequest> CreateRequestClient<TRequest>(this ConsumeContext consumeContext, IBus bus, RequestTimeout timeout = default)
            where TRequest : class
        {
            var clientFactory = new ClientFactory(new BusClientFactoryContext(bus, timeout));

            return clientFactory.CreateRequestClient<TRequest>(consumeContext, timeout);
        }

        /// <summary>
        /// Create a client factory from the bus, which uses the default bus endpoint for any response messages
        /// </summary>
        /// <param name="bus">THe bus instance</param>
        /// <param name="timeout">The default request timeout</param>
        /// <returns></returns>
        public static IClientFactory CreateClientFactory(this IBus bus, RequestTimeout timeout = default)
        {
            return new ClientFactory(new BusClientFactoryContext(bus, timeout));
        }

        /// <summary>
        /// Connects a client factory to a host receive endpoint, using the bus as the send endpoint provider
        /// </summary>
        /// <param name="receiveEndpoint"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static IClientFactory CreateClientFactory(this ReceiveEndpointReady receiveEndpoint, RequestTimeout timeout = default)
        {
            var context = new ReceiveEndpointClientFactoryContext(receiveEndpoint, timeout);

            return new ClientFactory(context);
        }

        /// <summary>
        /// Connects a client factory to a host receive endpoint, using the bus as the send endpoint provider
        /// </summary>
        /// <param name="receiveEndpointHandle">
        /// A handle to the receive endpoint, which is stopped when the client factory is disposed
        /// </param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<IClientFactory> CreateClientFactory(this HostReceiveEndpointHandle receiveEndpointHandle, RequestTimeout timeout = default)
        {
            var ready = await receiveEndpointHandle.Ready.ConfigureAwait(false);

            var context = new HostReceiveEndpointClientFactoryContext(receiveEndpointHandle, ready, timeout);

            return new ClientFactory(context);
        }

        /// <summary>
        /// Connects a new receive endpoint to the host, and creates a <see cref="IClientFactory" />.
        /// </summary>
        /// <param name="connector">The host to connect the new receive endpoint</param>
        /// <param name="timeout">The default request timeout</param>
        /// <returns></returns>
        public static Task<IClientFactory> CreateClientFactory(this IReceiveConnector connector, RequestTimeout timeout = default)
        {
            var receiveEndpointHandle = connector.ConnectResponseEndpoint();

            return receiveEndpointHandle.CreateClientFactory(timeout);
        }

        /// <summary>
        /// Connects a new receive endpoint to the host, and creates a <see cref="IClientFactory" />.
        /// </summary>
        /// <param name="connector">The host to connect the new receive endpoint</param>
        /// <param name="timeout">The default request timeout</param>
        /// <returns></returns>
        public static Task<IClientFactory> ConnectClientFactory(this IReceiveConnector connector, RequestTimeout timeout = default)
        {
            var endpointDefinition = new TemporaryEndpointDefinition();

            var receiveEndpointHandle = connector.ConnectReceiveEndpoint(endpointDefinition, KebabCaseEndpointNameFormatter.Instance);

            return receiveEndpointHandle.CreateClientFactory(timeout);
        }
    }
}
