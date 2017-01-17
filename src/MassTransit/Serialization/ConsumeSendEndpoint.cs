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
namespace MassTransit.Serialization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Util;


    /// <summary>
    /// Intercepts the ISendEndpoint and makes it part of the current consume context
    /// </summary>
    public class ConsumeSendEndpoint :
        ISendEndpoint
    {
        readonly ConsumeContext _context;
        readonly ISendEndpoint _endpoint;

        public ConsumeSendEndpoint(ISendEndpoint endpoint, ConsumeContext context)
        {
            _endpoint = endpoint;
            _context = context;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var sendContextPipe = new ConsumeSendContextPipe<T>(_context);

            var task = _endpoint.Send(message, sendContextPipe, cancellationToken);
            _context.ReceiveContext.AddPendingTask(task);
            return task;
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var sendContextPipe = new ConsumeSendContextPipe<T>(_context, pipe);

            var task = _endpoint.Send(message, sendContextPipe, cancellationToken);
            _context.ReceiveContext.AddPendingTask(task);
            return task;
        }

        public Task Send(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            Type messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            T message = TypeMetadataCache<T>.InitializeFromObject(values);

            return Send(message, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var sendContextPipe = new ConsumeSendContextPipe<T>(_context, pipe);

            var task = _endpoint.Send(message, sendContextPipe, cancellationToken);
            _context.ReceiveContext.AddPendingTask(task);
            return task;
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            Type messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            T message = TypeMetadataCache<T>.InitializeFromObject(values);

            return Send(message, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            T message = TypeMetadataCache<T>.InitializeFromObject(values);

            return Send(message, pipe, cancellationToken);
        }


        class ConsumeSendContextPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly ConsumeContext _context;
            readonly IPipe<SendContext<T>> _pipe;
            readonly IPipe<SendContext> _sendPipe;

            public ConsumeSendContextPipe(ConsumeContext context)
            {
                _context = context;
            }

            public ConsumeSendContextPipe(ConsumeContext context, IPipe<SendContext<T>> pipe)
            {
                _context = context;
                _pipe = pipe;
            }

            public ConsumeSendContextPipe(ConsumeContext context, IPipe<SendContext> pipe)
            {
                _context = context;
                _sendPipe = pipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
                _sendPipe?.Probe(context);
            }

            public async Task Send(SendContext<T> sendContext)
            {
                sendContext.SourceAddress = _context.ReceiveContext.InputAddress;

                if (_context.ConversationId.HasValue)
                    sendContext.ConversationId = _context.ConversationId;
                if (_context.CorrelationId.HasValue)
                    sendContext.InitiatorId = _context.CorrelationId;

                if (_pipe != null)
                    await _pipe.Send(sendContext).ConfigureAwait(false);
                if (_sendPipe != null)
                    await _sendPipe.Send(sendContext).ConfigureAwait(false);
            }
        }
    }
}