// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Initializers;
    using Util;


    public static class RequestClientExtensions
    {
        /// <summary>
        /// Send the request, and complete the response task when the response is received. If
        /// the request times out, a RequestTimeoutException is thrown. If the remote service 
        /// returns a fault, the task is set to exception status.
        /// </summary>
        /// <param name="client">The request client</param>
        /// <param name="values">The values to initialize the request object, anonymously</param>
        /// <param name="cancellationToken">A cancellation token for the request</param>
        /// <returns>The response Task</returns>
        public static async Task<TResponse> Request<TRequest, TResponse>(this IRequestClient<TRequest, TResponse> client, object values,
            CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : class
        {
            TRequest request = await MessageInitializerCache<TRequest>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            return await client.Request(request, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a request client that uses the bus to retrieve the endpoint and send the request.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="bus">The bus on which the client is created</param>
        /// <param name="address">The service address that handles the request</param>
        /// <param name="timeout">The timeout before the request is cancelled</param>
        /// <param name="ttl">THe time to live for the request message</param>
        /// <param name="callback">Callback when the request is sent</param>
        /// <returns></returns>
        public static IRequestClient<TRequest, TResponse> CreateRequestClient<TRequest, TResponse>(this IBus bus, Uri address, TimeSpan timeout,
            TimeSpan? ttl = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            return new MessageRequestClient<TRequest, TResponse>(bus, address, timeout, ttl, callback);
        }

        /// <summary>
        /// Creates a request client that uses the bus to publish a request.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="bus">The bus on which the client is created</param>
        /// <param name="timeout">The timeout before the request is cancelled</param>
        /// <param name="callback">Callback when the request is sent</param>
        /// <param name="timeToLive">The time that the request will live for</param>
        /// <returns></returns>
        public static IRequestClient<TRequest, TResponse> CreatePublishRequestClient<TRequest, TResponse>(this IBus bus, TimeSpan timeout,
            TimeSpan? timeToLive = default, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            return new PublishRequestClient<TRequest, TResponse>(bus, timeout, timeToLive, callback);
        }
    }
}
