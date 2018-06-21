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
namespace MassTransit.Pipeline.Filters.Outbox
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Util;


    public class OutboxSendEndpoint :
        ISendEndpoint
    {
        readonly ISendEndpoint _endpoint;
        readonly OutboxContext _outboxContext;

        /// <summary>
        /// Creates an send endpoint on the outbox
        /// </summary>
        /// <param name="outboxContext">The outbox context for this consume operation</param>
        /// <param name="endpoint">The actual endpoint returned by the transport</param>
        public OutboxSendEndpoint(OutboxContext outboxContext, ISendEndpoint endpoint)
        {
            _outboxContext = outboxContext;
            _endpoint = endpoint;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        Task ISendEndpoint.Send<T>(T message, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send(object message, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, messageType, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, messageType, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(object values, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send<T>(values, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(values, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send<T>(values, pipe, cancellationToken));

            return TaskUtil.Completed;
        }
    }
}