namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public static class RequestExtensions
    {
        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="bus">A started bus instance</param>
        /// <param name="destinationAddress">The service address</param>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this IBus bus, Uri destinationAddress, TRequest message,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            using RequestHandle<TRequest> requestHandle = bus
                .CreateRequestClient<TRequest>(destinationAddress, timeout)
                .Create(message, cancellationToken);

            if (callback != null)
                requestHandle.UseExecute(callback);

            return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="bus">A started bus instance</param>
        /// <param name="destinationAddress">The service address</param>
        /// <param name="values">The values used to initialize the request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this IBus bus, Uri destinationAddress, object values,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            using RequestHandle<TRequest> requestHandle = bus
                .CreateRequestClient<TRequest>(destinationAddress, timeout)
                .Create(values, cancellationToken);

            if (callback != null)
                requestHandle.UseExecute(callback);

            return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="bus">A started bus instance</param>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this IBus bus, TRequest message,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            using RequestHandle<TRequest> requestHandle = bus
                .CreateRequestClient<TRequest>(timeout)
                .Create(message, cancellationToken);

            if (callback != null)
                requestHandle.UseExecute(callback);

            return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="bus">A started bus instance</param>
        /// <param name="values">The values used to initialize the request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this IBus bus, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            using RequestHandle<TRequest> requestHandle = bus
                .CreateRequestClient<TRequest>(timeout)
                .Create(values, cancellationToken);

            if (callback != null)
                requestHandle.UseExecute(callback);

            return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="bus">A started bus instance</param>
        /// <param name="destinationAddress">The service address</param>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this ConsumeContext consumeContext, IBus bus, Uri destinationAddress,
            TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            using RequestHandle<TRequest> requestHandle = consumeContext
                .CreateRequestClient<TRequest>(bus, destinationAddress, timeout)
                .Create(message, cancellationToken);

            if (callback != null)
                requestHandle.UseExecute(callback);

            return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="bus">A started bus instance</param>
        /// <param name="destinationAddress">The service address</param>
        /// <param name="values">The values used to initialize the request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this ConsumeContext consumeContext, IBus bus, Uri destinationAddress,
            object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            using RequestHandle<TRequest> requestHandle = consumeContext
                .CreateRequestClient<TRequest>(bus, destinationAddress, timeout)
                .Create(values, cancellationToken);

            if (callback != null)
                requestHandle.UseExecute(callback);

            return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="bus">A started bus instance</param>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this ConsumeContext consumeContext, IBus bus, TRequest message,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            using RequestHandle<TRequest> requestHandle = consumeContext
                .CreateRequestClient<TRequest>(bus, timeout)
                .Create(message, cancellationToken);

            if (callback != null)
                requestHandle.UseExecute(callback);

            return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="bus">A started bus instance</param>
        /// <param name="values">The values used to initialize the request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext" /> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this ConsumeContext consumeContext, IBus bus, object values,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            using RequestHandle<TRequest> requestHandle = consumeContext
                .CreateRequestClient<TRequest>(bus, timeout)
                .Create(values, cancellationToken);

            if (callback != null)
                requestHandle.UseExecute(callback);

            return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
        }
    }
}
