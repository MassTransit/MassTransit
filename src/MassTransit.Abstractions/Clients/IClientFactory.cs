namespace MassTransit
{
    using System;
    using System.Threading;


    /// <summary>
    /// The client factory is used to create request clients
    /// </summary>
    public interface IClientFactory
    {
        ClientFactoryContext Context { get; }

        /// <summary>
        /// Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.
        /// </summary>
        /// <param name="destinationAddress">The destination service address</param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.
        /// </summary>
        /// <param name="consumeContext">The consumeContext currently being processed</param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.
        /// </summary>
        /// <param name="consumeContext">The consumeContext currently being processed</param>
        /// <param name="destinationAddress">The destination service address</param>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        RequestHandle<T> CreateRequest<T>(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.
        /// </summary>
        /// <param name="destinationAddress">The destination service address</param>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        RequestHandle<T> CreateRequest<T>(Uri destinationAddress, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.
        /// </summary>
        /// <param name="consumeContext">The consumeContext currently being processed</param>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, using the message specified. If a destinationAddress for the message cannot be found, the message will be published.
        /// </summary>
        /// <param name="consumeContext">The consumeContext currently being processed</param>
        /// <param name="destinationAddress">The destination service address</param>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken"></param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request client for the specified message type
        /// </summary>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request client for the specified message type
        /// </summary>
        /// <param name="consumeContext">The consumeContext currently being processed</param>
        /// <param name="timeout"></param>
        /// <typeparam name="T">The message type</typeparam>
        /// <returns></returns>
        IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request client, using the specified service address
        /// </summary>
        /// <param name="destinationAddress">The destination service address</param>
        /// <param name="timeout">The default timeout for requests</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request client, using the specified service address
        /// </summary>
        /// <param name="consumeContext">The consumeContext currently being processed</param>
        /// <param name="destinationAddress">The destination service address</param>
        /// <param name="timeout">The default timeout for requests</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout = default)
            where T : class;
    }
}
