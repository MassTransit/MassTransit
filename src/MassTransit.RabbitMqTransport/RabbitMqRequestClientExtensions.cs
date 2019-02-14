// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Clients;
    using Clients.Contexts;
    using RabbitMqTransport;


    public static class RabbitMqRequestClientExtensions
    {
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
