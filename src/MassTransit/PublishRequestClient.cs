namespace MassTransit
{
    using System;


    /// <summary>
    /// Implements a request client that allows a message to be published, versus sending to a specific endpoint
    /// </summary>
    /// <typeparam name="TRequest">The request message type</typeparam>
    /// <typeparam name="TResponse">The response message type</typeparam>
    [Obsolete("This will be deprecated in the next release")]
    public class PublishRequestClient<TRequest, TResponse> :
        MessageRequestClient<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Creates a message request client for the bus and endpoint specified
        /// </summary>
        /// <param name="bus">The bus instance</param>
        /// <param name="timeout">The request timeout</param>
        /// <param name="timeToLive">The time that the request will live for</param>
        /// <param name="callback"></param>
        public PublishRequestClient(IBus bus, TimeSpan timeout, TimeSpan? timeToLive = default, Action<SendContext<TRequest>> callback = null)
            : base(bus.CreateClientFactory(timeout).CreateRequestClient<TRequest>(), timeToLive, callback)
        {
        }

        /// <summary>
        /// Creates a message request client for the bus and endpoint specified
        /// </summary>
        /// <param name="client"></param>
        /// <param name="timeToLive">The time the request will live for</param>
        /// <param name="callback"></param>
        public PublishRequestClient(IRequestClient<TRequest> client, TimeSpan? timeToLive = default, Action<SendContext<TRequest>> callback = null)
            : base(client, timeToLive, callback)
        {
        }
    }
}
