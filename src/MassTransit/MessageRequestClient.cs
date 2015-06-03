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
    using Context;


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
        readonly IBus _bus;
        readonly Lazy<Task<ISendEndpoint>> _requestEndpoint;
        readonly TimeSpan _timeout;

        /// <summary>
        /// Creates a message request client for the bus and endpoint specified
        /// </summary>
        /// <param name="bus">The bus instance</param>
        /// <param name="address">The service endpoint address</param>
        /// <param name="timeout">The request timeout</param>
        public MessageRequestClient(IBus bus, Uri address, TimeSpan timeout)
        {
            _bus = bus;
            _timeout = timeout;
            _requestEndpoint = new Lazy<Task<ISendEndpoint>>(async () => await _bus.GetSendEndpoint(address));
        }

        async Task<TResponse> IRequestClient<TRequest, TResponse>.Request(TRequest request, CancellationToken cancellationToken)
        {
            var taskScheduler = SynchronizationContext.Current == null
                ? TaskScheduler.Default
                : TaskScheduler.FromCurrentSynchronizationContext();

            Task<TResponse> responseTask = null;
            var pipe = new SendRequest<TRequest>(_bus, taskScheduler, x =>
            {
                x.Timeout = _timeout;

                responseTask = x.Handle<TResponse>();
            });

            ISendEndpoint endpoint = await _requestEndpoint.Value;

            await endpoint.Send(request, pipe, cancellationToken);

            return await responseTask;
        }
    }
}