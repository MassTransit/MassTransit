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


    /// <summary>
    /// Implements a request client that uses specified endpoint to send the request and return the
    /// response task.
    /// </summary>
    /// <typeparam name="TRequest">The request message type</typeparam>
    /// <typeparam name="TResponse">The response message type</typeparam>
    public class MessageRequestClient<TRequest, TResponse> :
        IRequestClient<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        readonly TimeSpan? _timeToLive;
        readonly Action<SendContext<TRequest>> _callback;
        readonly IRequestClient<TRequest> _client;

        /// <summary>
        /// Creates a message request client for the bus and endpoint specified
        /// </summary>
        /// <param name="bus">The bus instance</param>
        /// <param name="serviceAddress">The service endpoint address</param>
        /// <param name="timeout">The request timeout</param>
        /// <param name="timeToLive">The time that the request will live for</param>
        /// <param name="callback"></param>
        public MessageRequestClient(IBus bus, Uri serviceAddress, TimeSpan timeout, TimeSpan? timeToLive = default,
            Action<SendContext<TRequest>> callback = null)
        {
            _timeToLive = timeToLive;
            _callback = callback;

            var clientFactory = bus.CreateClientFactory(timeout);

            _client = clientFactory.CreateRequestClient<TRequest>(serviceAddress);
        }

        /// <summary>
        /// Creates a message request client for the bus and endpoint specified
        /// </summary>
        /// <param name="requestClient"></param>
        /// <param name="timeToLive">The time that the request will live for</param>
        /// <param name="callback"></param>
        public MessageRequestClient(IRequestClient<TRequest> requestClient, TimeSpan? timeToLive = default,
            Action<SendContext<TRequest>> callback = null)
        {
            _callback = callback;
            _client = requestClient;
            _timeToLive = timeToLive;
        }

        public async Task<TResponse> Request(TRequest request, CancellationToken cancellationToken = default)
        {
            using (var requestHandle = _client.Create(request, cancellationToken))
            {
                if (_timeToLive.HasValue)
                    requestHandle.TimeToLive = _timeToLive.Value;

                if (_callback != null)
                    requestHandle.UseExecute(_callback);

                Response<TResponse> response = await requestHandle.GetResponse<TResponse>().ConfigureAwait(false);
                
                return response.Message;
            }
        }
    }
}