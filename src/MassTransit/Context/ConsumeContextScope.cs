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
namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;


    /// <summary>
    /// A consume context proxy creates a payload scope, such that anything added to the payload
    /// of the context is only added at the scope level and below.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumeContextScope<TMessage> :
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly PayloadCache _payloadCache;

        public ConsumeContextScope(ConsumeContext<TMessage> context)
        {
            _context = context;
            _payloadCache = new PayloadCache();
        }

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public CancellationToken CancellationToken => _context.CancellationToken;

        public bool HasPayloadType(Type contextType)
        {
            return _payloadCache.HasPayloadType(contextType) || _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context)
            where TPayload : class
        {
            if (_payloadCache.TryGetPayload(out context))
                return true;

            return _context.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            TPayload payload;
            if (_payloadCache.TryGetPayload(out payload))
                return payload;

            if (_context.TryGetPayload(out payload))
                return payload;

            return _payloadCache.GetOrAddPayload(payloadFactory);
        }

        Guid? MessageContext.MessageId => _context.MessageId;

        public Guid? RequestId => _context.RequestId;

        public Guid? CorrelationId => _context.CorrelationId;

        public DateTime? ExpirationTime => _context.ExpirationTime;

        public Uri SourceAddress => _context.SourceAddress;

        public Uri DestinationAddress => _context.DestinationAddress;

        public Uri ResponseAddress => _context.ResponseAddress;

        Uri MessageContext.FaultAddress => _context.FaultAddress;

        Headers MessageContext.Headers => _context.Headers;

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
        }

        ReceiveContext ConsumeContext.ReceiveContext => _context.ReceiveContext;

        Task ConsumeContext.CompleteTask => _context.CompleteTask;

        IEnumerable<string> ConsumeContext.SupportedMessageTypes => _context.SupportedMessageTypes;

        bool ConsumeContext.HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        bool ConsumeContext.TryGetMessage<T>(out ConsumeContext<T> consumeContext)
        {
            return _context.TryGetMessage(out consumeContext);
        }

        Task ConsumeContext.RespondAsync<T>(T message)
        {
            return _context.RespondAsync(message);
        }

        void ConsumeContext.Respond<T>(T message)
        {
            _context.Respond(message);
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        Task ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        Task ConsumeContext.NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        TMessage ConsumeContext<TMessage>.Message => _context.Message;

        Task ConsumeContext<TMessage>.NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(duration, consumerType);
        }

        Task ConsumeContext<TMessage>.NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(duration, consumerType, exception);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }
    }
}