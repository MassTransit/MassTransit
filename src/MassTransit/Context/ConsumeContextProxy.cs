// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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


    public class ConsumeContextProxy<T> :
        ConsumeContext<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;
        readonly PayloadCache _payloadCache;

        public ConsumeContextProxy(ConsumeContext<T> context)
        {
            _context = context;
            _payloadCache = new PayloadCache();
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

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

        public Guid? MessageId
        {
            get { return _context.MessageId; }
        }

        public Guid? RequestId
        {
            get { return _context.RequestId; }
        }

        public Guid? CorrelationId
        {
            get { return _context.CorrelationId; }
        }

        public DateTime? ExpirationTime
        {
            get { return _context.ExpirationTime; }
        }

        public Uri SourceAddress
        {
            get { return _context.SourceAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _context.DestinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _context.ResponseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _context.FaultAddress; }
        }

        public ContextHeaders ContextHeaders
        {
            get { return _context.ContextHeaders; }
        }

        public Task Publish<T1>(T1 message, CancellationToken cancellationToken = new CancellationToken()) where T1 : class
        {
            return _context.Publish(message, cancellationToken);
        }

        public Task Publish<T1>(T1 message, IPipe<PublishContext<T1>> publishPipe,
            CancellationToken cancellationToken = new CancellationToken()) where T1 : class
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish<T1>(T1 message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
            where T1 : class
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish(object message, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Publish(message, cancellationToken);
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Publish(message, messageType, cancellationToken);
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish<T1>(object values, CancellationToken cancellationToken = new CancellationToken()) where T1 : class
        {
            return _context.Publish<T1>(values, cancellationToken);
        }

        public Task Publish<T1>(object values, IPipe<PublishContext<T1>> publishPipe,
            CancellationToken cancellationToken = new CancellationToken()) where T1 : class
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        public Task Publish<T1>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken = new CancellationToken()) where T1 : class
        {
            return _context.Publish<T1>(values, publishPipe, cancellationToken);
        }

        public ReceiveContext ReceiveContext
        {
            get { return _context.ReceiveContext; }
        }

        public Task CompleteTask
        {
            get { return _context.CompleteTask; }
        }

        public IEnumerable<string> SupportedMessageTypes
        {
            get { return _context.SupportedMessageTypes; }
        }

        public bool HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        public bool TryGetMessage<T1>(out ConsumeContext<T1> consumeContext) where T1 : class
        {
            return _context.TryGetMessage(out consumeContext);
        }

        public Task RespondAsync<T1>(T1 message) where T1 : class
        {
            return _context.RespondAsync(message);
        }

        public void Respond<T1>(T1 message) where T1 : class
        {
            _context.Respond(message);
        }

        public void RetryLater()
        {
            _context.RetryLater();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
        {
            _context.NotifyConsumed(elapsed, messageType, consumerType);
        }

        public void NotifyFaulted<T1>(T1 message, string consumerType, Exception exception) where T1 : class
        {
            _context.NotifyFaulted(message, consumerType, exception);
        }

        public T Message
        {
            get { return _context.Message; }
        }

        public void NotifyConsumed(TimeSpan elapsed, string consumerType)
        {
            _context.NotifyConsumed(elapsed, consumerType);
        }

        public void NotifyFaulted(string consumerType, Exception exception)
        {
            _context.NotifyFaulted(consumerType, exception);
        }
    }
}