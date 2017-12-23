// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using AzureServiceBusTransport;


    public static class ServiceBusRequestClientExtensions
    {
        /// <summary>
        /// Creates a request client that uses the bus to retrieve the endpoint and send the request.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="host"></param>
        /// <param name="sendEndpointProvider"></param>
        /// <param name="address">The service address that handles the request</param>
        /// <param name="timeout">The timeout before the request is cancelled</param>
        /// <param name="ttl">THe time to live for the request message</param>
        /// <param name="callback">Callback when the request is sent</param>
        /// <returns></returns>
        public static async Task<IRequestClient<TRequest, TResponse>> CreateRequestClient<TRequest, TResponse>(this IServiceBusHost host,
            ISendEndpointProvider sendEndpointProvider, Uri address, TimeSpan timeout, TimeSpan? ttl = default(TimeSpan?),
            Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var endpoint = host.ConnectReceiveEndpoint(host.GetTemporaryQueueName("response"));

            var ready = await endpoint.Ready.ConfigureAwait(false);

            return new MessageRequestClient<TRequest, TResponse>(sendEndpointProvider, ready.ReceiveEndpoint, ready.InputAddress, address, timeout, ttl,
                callback);
        }

        /// <summary>
        /// Creates a request client factory which can be used to create a request client per message within a consume context.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="host">The host for the response endpoint</param>
        /// <param name="address">The service address</param>
        /// <param name="timeout">The request timeout</param>
        /// <param name="timeToLive">The request time to live</param>
        /// <param name="callback">Customize the send context</param>
        /// <returns></returns>
        public static async Task<IRequestClientFactory<TRequest, TResponse>> CreateRequestClientFactory<TRequest, TResponse>(this IServiceBusHost host,
            Uri address, TimeSpan timeout, TimeSpan? timeToLive = default(TimeSpan?), Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var endpoint = host.ConnectReceiveEndpoint(host.GetTemporaryQueueName("response"));

            var ready = await endpoint.Ready.ConfigureAwait(false);

            return new MessageRequestClientFactory<TRequest, TResponse>(endpoint, ready.ReceiveEndpoint, ready.InputAddress, address, timeout, timeToLive, callback);
        }

        /// <summary>
        /// Creates a request client that uses the bus to publish a request.
        /// </summary>
        /// <typeparam name="TRequest">The request type</typeparam>
        /// <typeparam name="TResponse">The response type</typeparam>
        /// <param name="timeout">The timeout before the request is cancelled</param>
        /// <param name="callback">Callback when the request is sent</param>
        /// <param name="ttl">The time that the request will live for</param>
        /// <param name="host"></param>
        /// <param name="publishEndpoint"></param>
        /// <returns></returns>
        public static async Task<IRequestClient<TRequest, TResponse>> CreatePublishRequestClient<TRequest, TResponse>(this IServiceBusHost host,
            IPublishEndpoint publishEndpoint, TimeSpan timeout, TimeSpan? ttl = default(TimeSpan?), Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var endpoint = host.ConnectReceiveEndpoint(host.GetTemporaryQueueName("response"));

            var ready = await endpoint.Ready.ConfigureAwait(false);

            return new PublishRequestClient<TRequest, TResponse>(publishEndpoint, ready.ReceiveEndpoint, ready.InputAddress, timeout, ttl, callback);
        }
    }
}