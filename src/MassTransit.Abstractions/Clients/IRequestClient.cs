namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A request client, which is used to send a request, as well as get one or more response types from that request.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    public interface IRequestClient<TRequest>
        where TRequest : class
    {
        /// <summary>
        /// Create a request, returning a <see cref="RequestHandle{TRequest}" />, which is then used to get responses, and ultimately
        /// send the request.
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <returns>A <see cref="RequestHandle{TRequest}" /> for the request</returns>
        RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default);

        /// <summary>
        /// Create a request, returning a <see cref="RequestHandle{TRequest}" />, which is then used to get responses, and ultimately
        /// send the request.
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <returns>A <see cref="RequestHandle{TRequest}" /> for the request</returns>
        RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default);

        /// <summary>
        /// Create a request, and return a task for the specified response type
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, and return a task for the specified response type
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<Response<T>> GetResponse<T>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, and return a task for the specified response type
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, and return a task for the specified response type
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<Response<T>> GetResponse<T>(object values, RequestPipeConfiguratorCallback<TRequest> callback, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <returns></returns>
        Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <returns></returns>
        Task<Response<T1, T2>> GetResponse<T1, T2>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <returns></returns>
        Task<Response<T1, T2>> GetResponse<T1, T2>(object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <returns></returns>
        Task<Response<T1, T2>> GetResponse<T1, T2>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
            where T3 : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(TRequest message, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
            where T3 : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
            where T3 : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">
        /// An optional timeout, to automatically cancel the request after the specified timeout period
        /// </param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <returns></returns>
        Task<Response<T1, T2, T3>> GetResponse<T1, T2, T3>(object values, RequestPipeConfiguratorCallback<TRequest> callback,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T1 : class
            where T2 : class
            where T3 : class;
    }
}
