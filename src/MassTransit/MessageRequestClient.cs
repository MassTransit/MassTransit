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
    using Context;
    using Pipeline;


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
        readonly Action<SendContext<TRequest>> _callback;
        readonly IRequestPipeConnector _connector;
        readonly Lazy<Task<ISendEndpoint>> _requestEndpoint;
        readonly Uri _responseAddress;
        readonly TimeSpan _timeout;
        readonly TimeSpan? _timeToLive;

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
        {
            _connector = bus;
            _responseAddress = bus.Address;
            _timeout = timeout;
            _timeToLive = timeToLive;
            _callback = callback;

            _requestEndpoint = new Lazy<Task<ISendEndpoint>>(async () => await bus.GetSendEndpoint(serviceAddress).ConfigureAwait(false));
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
        {
            _connector = connector;
            _responseAddress = responseAddress;
            _timeout = timeout;
            _timeToLive = timeToLive;
            _callback = callback;

            _requestEndpoint = new Lazy<Task<ISendEndpoint>>(async () => await sendEndpointProvider.GetSendEndpoint(serviceAddress).ConfigureAwait(false));
        }

        public async Task<TResponse> Request(TRequest request, CancellationToken cancellationToken)
        {
            var taskScheduler = SynchronizationContext.Current == null
                ? TaskScheduler.Default
                : TaskScheduler.FromCurrentSynchronizationContext();

            Task<TResponse> responseTask = null;
            var pipe = new SendRequest<TRequest>(_connector, _responseAddress, taskScheduler, x =>
            {
                x.TimeToLive = _timeToLive;
                x.Timeout = _timeout;
                responseTask = x.Handle<TResponse>();

                _callback?.Invoke(x);
            });

            var endpoint = await _requestEndpoint.Value.ConfigureAwait(false);

            await endpoint.Send(request, pipe, cancellationToken).ConfigureAwait(false);

            return await responseTask.ConfigureAwait(false);
        }
    }
}