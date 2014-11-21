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
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Pipeline;
    using Util;


    public class JsonMessageConsumeContext<TMessage> :
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext _context;
        readonly TMessage _message;
        public IEnumerable<string> SupportedMessageTypes
        {
            get { return _context.SupportedMessageTypes; }
        }

        public JsonMessageConsumeContext(ConsumeContext context, TMessage message)
        {
            _context = context;
            _message = message;
        }

        public Task Publish<T>(T message) where T : class
        {
            return _context.Publish(message);
        }

        public Task Publish<T>(T message, IPipe<MassTransit.PublishContext<T>> publishPipe) where T : class
        {
            return _context.Publish(message, publishPipe);
        }

        public Task Publish(object message)
        {
            return _context.Publish(message);
        }

        public Task Publish(object message, Type messageType)
        {
            return _context.Publish(message, messageType);
        }

        public Task Publish(object message, Action<PublishContext> contextCallback)
        {
            return _context.Publish(message, contextCallback);
        }

        public Task Publish(object message, Type messageType, Action<PublishContext> contextCallback)
        {
            return _context.Publish(message, messageType, contextCallback);
        }

        public Task Publish<T>(object values) where T : class
        {
            return _context.Publish<T>(values);
        }

        public Task Publish<T>(object values, Action<MassTransit.PublishContext<T>> contextCallback) where T : class
        {
            return _context.Publish(values, contextCallback);
        }

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload)
            where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
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

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public ReceiveContext ReceiveContext
        {
            get { return _context.ReceiveContext; }
        }

        public bool HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            return _context.TryGetMessage(out consumeContext);
        }

        public Task RespondAsync<T>(T message)
            where T : class
        {
            return _context.RespondAsync(message);
        }

        public void Respond<T>(T message)
            where T : class
        {
            _context.Respond(message);
        }

        public void RetryLater()
        {
            _context.RetryLater();
        }

        public ISendEndpoint GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
        {
            _context.NotifyConsumed(elapsed, messageType, consumerType);
        }

        public void NotifyFaulted(string messageType, string consumerType, Exception exception)
        {
            _context.NotifyFaulted(messageType, consumerType, exception);
        }

        public TMessage Message
        {
            get { return _message; }
        }

        public void NotifyConsumed(TimeSpan elapsed, string consumerType)
        {
            _context.NotifyConsumed(elapsed, TypeMetadataCache<TMessage>.ShortName, consumerType);
        }

        public void NotifyFaulted(string consumerType, Exception exception)
        {
            _context.NotifyFaulted(TypeMetadataCache<TMessage>.ShortName, consumerType, exception);
        }
    }
}