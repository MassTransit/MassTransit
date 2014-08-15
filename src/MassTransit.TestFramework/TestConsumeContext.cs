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
namespace MassTransit.TestFramework
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Pipeline;


    public class TestConsumeContext<T> :
        ConsumeContext<T>
        where T : class
    {
        PayloadCache _cache = new PayloadCache();
        CancellationToken _cancellationToken;
        ContextHeaders _contextHeaders;
        Guid? _correlationId;
        Uri _destinationAddress;
        DateTime? _expirationTime;
        Uri _faultAddress;
        T _message;
        Guid? _messageId;
        ReceiveContext _receiveContext;
        Guid? _requestId;
        Uri _responseAddress;
        Uri _sourceAddress;

        public TestConsumeContext(T message)
        {
            _message = message;

            _cancellationToken = new CancellationToken();

            _messageId = NewId.NextGuid();
            _sourceAddress = new Uri("loopback://localhost/input_queue");
            _destinationAddress = new Uri("loopback://localhost/input_queue");
        }

        public bool HasPayloadType(Type contextType)
        {
            return _cache.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload context) where TPayload : class
        {
            return _cache.TryGetPayload(out context);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _cache.GetOrAddPayload(payloadFactory);
        }

        public CancellationToken CancellationToken
        {
            get { return _cancellationToken; }
        }

        public Guid? MessageId
        {
            get { return _messageId; }
        }

        public Guid? RequestId
        {
            get { return _requestId; }
        }

        public Guid? CorrelationId
        {
            get { return _correlationId; }
        }

        public DateTime? ExpirationTime
        {
            get { return _expirationTime; }
        }

        public Uri SourceAddress
        {
            get { return _sourceAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _destinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _responseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _faultAddress; }
        }

        public ContextHeaders ContextHeaders
        {
            get { return _contextHeaders; }
        }

        public Task Publish<TMessage>(TMessage message) where TMessage : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<TMessage>(TMessage message, IPipe<MassTransit.PublishContext<TMessage>> publishPipe) where TMessage : class
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Type messageType)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Action<PublishContext> contextCallback)
        {
            throw new NotImplementedException();
        }

        public Task Publish(object message, Type messageType, Action<PublishContext> contextCallback)
        {
            throw new NotImplementedException();
        }

        public Task Publish<TMessage>(object values) where TMessage : class
        {
            throw new NotImplementedException();
        }

        public Task Publish<TMessage>(object values, Action<MassTransit.PublishContext<TMessage>> contextCallback) where TMessage : class
        {
            throw new NotImplementedException();
        }

        public ReceiveContext ReceiveContext
        {
            get { return _receiveContext; }
        }

        public bool HasMessageType(Type messageType)
        {
            throw new NotImplementedException();
        }

        public bool TryGetMessage<TMessage>(out ConsumeContext<TMessage> consumeContext)
            where TMessage : class
        {
            consumeContext = this as ConsumeContext<TMessage>;
            return consumeContext != null;
        }

        public Task RespondAsync<T1>(T1 message) where T1 : class
        {
            throw new NotImplementedException();
        }

        public void Respond<T1>(T1 message) where T1 : class
        {
            throw new NotImplementedException();
        }

        public void RetryLater()
        {
            throw new NotImplementedException();
        }

        public ISendEndpoint GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public void NotifyConsumed(TimeSpan elapsed, string messageType, string consumerType)
        {
        }

        public void NotifyFaulted(string messageType, string consumerType, Exception exception)
        {
        }

        public T Message
        {
            get { return _message; }
        }

        public void NotifyConsumed(TimeSpan elapsed, string consumerType)
        {
        }

        public void NotifyFaulted(string consumerType, Exception exception)
        {
        }
    }
}