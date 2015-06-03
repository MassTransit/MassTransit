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


    public static class RequestExtensions
    {
        /// <summary>
        /// Send a request from the bus to the endpoint, establishing response handlers
        /// </summary>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <param name="bus">The bus instance</param>
        /// <param name="address">The service endpoint address</param>
        /// <param name="message">The request message</param>
        /// <param name="callback">A callback to configure the request and response handlers</param>
        /// <param name="cancellationToken">Can be used to cancel the request</param>
        /// <returns>An awaitable task that completes once the request is sent</returns>
        public static async Task<Request<TRequest>> Request<TRequest>(this IBus bus, Uri address, TRequest message,
            Action<RequestContext<TRequest>> callback, CancellationToken cancellationToken = default(CancellationToken))
            where TRequest : class
        {
            TaskScheduler taskScheduler = SynchronizationContext.Current == null
                ? TaskScheduler.Default
                : TaskScheduler.FromCurrentSynchronizationContext();

            ISendEndpoint endpoint = await bus.GetSendEndpoint(address).ConfigureAwait(false);

            var pipe = new SendRequest<TRequest>(bus, taskScheduler, callback);

            await endpoint.Send(message, pipe, cancellationToken).ConfigureAwait(false);

            return pipe;
        }

        /// <summary>
        /// Send a request from the bus to the endpoint, establishing response handlers
        /// </summary>
        /// <typeparam name="TRequest">The request message type</typeparam>
        /// <param name="bus">The bus instance</param>
        /// <param name="sendEndpoint">The service endpoint</param>
        /// <param name="message">The request message</param>
        /// <param name="callback">A callback to configure the request and response handlers</param>
        /// <param name="cancellationToken">Can be used to cancel the request</param>
        /// <returns>An awaitable task that completes once the request is sent</returns>
        public static async Task<Request<TRequest>> Request<TRequest>(this IBus bus, ISendEndpoint sendEndpoint, TRequest message,
            Action<RequestContext<TRequest>> callback, CancellationToken cancellationToken = default(CancellationToken))
            where TRequest : class
        {
            TaskScheduler taskScheduler = SynchronizationContext.Current == null
                ? TaskScheduler.Default
                : TaskScheduler.FromCurrentSynchronizationContext();

            var pipe = new SendRequest<TRequest>(bus, taskScheduler, callback);

            await sendEndpoint.Send(message, pipe, cancellationToken).ConfigureAwait(false);

            return pipe;
        }
    }
}