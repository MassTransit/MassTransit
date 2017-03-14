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
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;


    /// <summary>
    /// Implements a request client that uses specified endpoint to send the request and return the
    /// response task.
    /// </summary>
    /// <typeparam name="TRequest">The request message type</typeparam>
    /// <typeparam name="TResponse">The response message type</typeparam>
    public class MessageRequestClient<TRequest, TResponse> :
        RequestClient<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly Uri _serviceAddress;

        /// <summary>
        /// Creates a message request client for the bus and endpoint specified
        /// </summary>
        /// <param name="bus">The bus instance</param>
        /// <param name="serviceAddress">The service endpoint address</param>
        /// <param name="timeout">The request timeout</param>
        /// <param name="timeToLive">The time that the request will live for</param>
        /// <param name="callback"></param>
        public MessageRequestClient(IBus bus, Uri serviceAddress, TimeSpan timeout, TimeSpan? timeToLive = default(TimeSpan?),
            Action<SendContext<TRequest>> callback = null)
            : base(bus, bus.Address, timeout, timeToLive, callback)
        {
            _sendEndpointProvider = bus;
            _serviceAddress = serviceAddress;
        }

        /// <summary>
        /// Creates a message request client for the bus and endpoint specified
        /// </summary>
        /// <param name="sendEndpointProvider"></param>
        /// <param name="connector">The bus instance</param>
        /// <param name="responseAddress">The response address of the connector</param>
        /// <param name="serviceAddress">The service endpoint address</param>
        /// <param name="timeout">The request timeout</param>
        /// <param name="timeToLive">The time that the request will live for</param>
        /// <param name="callback"></param>
        public MessageRequestClient(ISendEndpointProvider sendEndpointProvider, IRequestPipeConnector connector, Uri responseAddress, Uri serviceAddress,
            TimeSpan timeout, TimeSpan? timeToLive = default(TimeSpan?), Action<SendContext<TRequest>> callback = null)
            : base(connector, responseAddress, timeout, timeToLive, callback)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _serviceAddress = serviceAddress;
        }

        protected override async Task SendRequest(TRequest request, IPipe<SendContext<TRequest>> requestPipe, CancellationToken cancellationToken)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(_serviceAddress).ConfigureAwait(false);

            await endpoint.Send(request, requestPipe, cancellationToken).ConfigureAwait(false);
        }
    }
}