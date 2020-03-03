namespace MassTransit
{
    using System;
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
        /// Create a request, returning a <see cref="RequestHandle{TRequest}"/>, which is then used to get responses, and ultimately
        /// send the request.
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">An optional timeout, to automatically cancel the request after the specified timeout period</param>
        /// <returns>A <see cref="RequestHandle{TRequest}"/> for the request</returns>
        RequestHandle<TRequest> Create(TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default);

        /// <summary>
        /// Create a request, returning a <see cref="RequestHandle{TRequest}"/>, which is then used to get responses, and ultimately
        /// send the request.
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">An optional timeout, to automatically cancel the request after the specified timeout period</param>
        /// <returns>A <see cref="RequestHandle{TRequest}"/> for the request</returns>
        RequestHandle<TRequest> Create(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default);

        /// <summary>
        /// Create a request, and return a task for the specified response type
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">An optional timeout, to automatically cancel the request after the specified timeout period</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<Response<T>> GetResponse<T>(TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, and return a task for the specified response type
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">An optional timeout, to automatically cancel the request after the specified timeout period</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<Response<T>> GetResponse<T>(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">An optional timeout, to automatically cancel the request after the specified timeout period</param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <returns></returns>
        Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(TRequest message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// Create a request, and return a task for the specified response types
        /// </summary>
        /// <param name="values">The values to initialize the message</param>
        /// <param name="cancellationToken">An optional cancellationToken to cancel the request</param>
        /// <param name="timeout">An optional timeout, to automatically cancel the request after the specified timeout period</param>
        /// <typeparam name="T1">The first response type</typeparam>
        /// <typeparam name="T2">The second response type</typeparam>
        /// <returns></returns>
        Task<(Task<Response<T1>>, Task<Response<T2>>)> GetResponse<T1, T2>(object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T1 : class
            where T2 : class;
    }


    /// <summary>
    /// The legacy request client interface, which combines the request and response type into a single interface. This will eventually
    /// be marked obsolete.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    [Obsolete("This will be deprecated in the next release")]
    public interface IRequestClient<in TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Send the request, and complete the response task when the response is received. If
        /// the request times out, a RequestTimeoutException is thrown. If the remote service
        /// returns a fault, the task is set to exception status.
        /// </summary>
        /// <param name="request">The request message</param>
        /// <param name="cancellationToken">A cancellation token for the request</param>
        /// <returns>The response Task</returns>
        Task<TResponse> Request(TRequest request, CancellationToken cancellationToken = default);
    }
}
