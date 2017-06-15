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
namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Payloads;
    using GreenPipes.Util;
    using Pipeline;
    using Pipeline.Observables;
    using Transports;
    using Transports.InMemory;
    using Util;


    public class TestConsumeContext<TMessage> :
        BasePipeContext,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        Task _completeTask;
        Guid? _conversationId;
        Guid? _correlationId;
        Guid? _initiatorId;
        Uri _destinationAddress;
        DateTime? _expirationTime;
        Uri _faultAddress;
        Headers _headers;
        TMessage _message;
        Guid? _messageId;
        ReceiveContext _receiveContext;
        Guid? _requestId;
        Uri _responseAddress;
        Uri _sourceAddress;

        public TestConsumeContext(TMessage message)
            : base(new PayloadCache())
        {
            _message = message;

            _messageId = NewId.NextGuid();
            _sourceAddress = new Uri("loopback://localhost/input_queue");
            _destinationAddress = new Uri("loopback://localhost/input_queue");

            _receiveContext = new TestReceiveContext(_sourceAddress);
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

        Guid? MessageContext.ConversationId => _conversationId;

        public Guid? InitiatorId
        {
            get { return _initiatorId; }
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

        public Headers Headers
        {
            get { return _headers; }
        }

        public HostInfo Host
        {
            get { return null; }
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public ReceiveContext ReceiveContext
        {
            get { return _receiveContext; }
        }

        public Task CompleteTask
        {
            get { return Task.FromResult(true); }
        }

        public IEnumerable<string> SupportedMessageTypes
        {
            get { return Enumerable.Repeat(new MessageUrn(typeof(TMessage)).ToString(), 1); }
        }

        public bool HasMessageType(Type messageType)
        {
            return messageType.GetTypeInfo().IsAssignableFrom(typeof(TMessage));
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            consumeContext = this as ConsumeContext<T>;
            return consumeContext != null;
        }

        public Task RespondAsync<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe) where T : class
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, Type messageType)
        {
            throw new NotImplementedException();
        }

        public Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            throw new NotImplementedException();
        }

        public void Respond<T>(T message) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            throw new NotImplementedException();
        }

        public async Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
        }

        public async Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
        }

        public TMessage Message
        {
            get { return _message; }
        }

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return NotifyConsumed(this, duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(this, duration, consumerType, exception);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return new Connectable<IPublishObserver>().Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return new Connectable<ISendObserver>().Connect(observer);
        }
    }


    public class TestReceiveContext : 
        BaseReceiveContext
    {
        public TestReceiveContext(Uri sourceAddress)
            : base(sourceAddress, false, new ReceiveObservable(), null)
        {
            HeaderProvider = new DictionaryHeaderProvider(new Dictionary<string, object>());
        }

        protected override IHeaderProvider HeaderProvider { get; }
        protected override Stream GetBodyStream()
        {
            return new MemoryStream();
        }
    }
}