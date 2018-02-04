// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Initializers;
    using Util;


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
        /// <param name="callback">A callback, which can modify the <see cref="SendContext"/> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this IBus bus, Uri destinationAddress, TRequest message,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var requestClient = bus.CreateRequestClient<TRequest>(destinationAddress, timeout);

            using (var requestHandle = requestClient.Create(message, cancellationToken))
            {
                if (callback != null)
                    requestHandle.UseExecute(callback);

                return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="bus">A started bus instance</param>
        /// <param name="destinationAddress">The service address</param>
        /// <param name="values">The values used to initialize the request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext"/> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this IBus bus, Uri destinationAddress, object values,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            TRequest message = await MessageInitializerCache<TRequest>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            return await Request<TRequest, TResponse>(bus, destinationAddress, message, cancellationToken, timeout, callback).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="bus">A started bus instance</param>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext"/> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this IBus bus, TRequest message,
            CancellationToken cancellationToken = default,
            RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var requestClient = bus.CreateRequestClient<TRequest>(timeout);

            using (var requestHandle = requestClient.Create(message, cancellationToken))
            {
                if (callback != null)
                    requestHandle.UseExecute(callback);

                return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="bus">A started bus instance</param>
        /// <param name="values">The values used to initialize the request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext"/> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this IBus bus, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            TRequest message = await MessageInitializerCache<TRequest>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            return await Request<TRequest, TResponse>(bus, message, cancellationToken, timeout, callback).ConfigureAwait(false);
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
        /// <param name="callback">A callback, which can modify the <see cref="SendContext"/> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this ConsumeContext consumeContext, IBus bus, Uri destinationAddress,
            TRequest message, CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var requestClient = consumeContext.CreateRequestClient<TRequest>(bus, destinationAddress, timeout);

            using (var requestHandle = requestClient.Create(message, cancellationToken))
            {
                if (callback != null)
                    requestHandle.UseExecute(callback);

                return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
            }
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
        /// <param name="callback">A callback, which can modify the <see cref="SendContext"/> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this ConsumeContext consumeContext, IBus bus, Uri destinationAddress,
            object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            TRequest message = await MessageInitializerCache<TRequest>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            return await Request<TRequest, TResponse>(consumeContext, bus, destinationAddress, message, cancellationToken, timeout, callback).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="bus">A started bus instance</param>
        /// <param name="message">The request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext"/> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this ConsumeContext consumeContext, IBus bus, TRequest message,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var requestClient = consumeContext.CreateRequestClient<TRequest>(bus, timeout);

            using (var requestHandle = requestClient.Create(message, cancellationToken))
            {
                if (callback != null)
                    requestHandle.UseExecute(callback);

                return await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, and return a Task which can be awaited for the response.
        /// </summary>
        /// <param name="consumeContext"></param>
        /// <param name="bus">A started bus instance</param>
        /// <param name="values">The values used to initialize the request message</param>
        /// <param name="cancellationToken">An optional cancellationToken for this request</param>
        /// <param name="timeout">An optional timeout for the request (defaults to 30 seconds)</param>
        /// <param name="callback">A callback, which can modify the <see cref="SendContext"/> of the request</param>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <returns></returns>
        public static async Task<Response<TResponse>> Request<TRequest, TResponse>(this ConsumeContext consumeContext, IBus bus, object values,
            CancellationToken cancellationToken = default, RequestTimeout timeout = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            TRequest message = await MessageInitializerCache<TRequest>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            return await Request<TRequest, TResponse>(consumeContext, bus, message, cancellationToken, timeout, callback).ConfigureAwait(false);
        }
    }
}
