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
namespace MassTransit.Steward.Core.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Contracts;
    using Pipeline;


    public class ConsumerDispatchContext :
        DispatchContextBase,
        DispatchContext
    {
        readonly string _body;
        readonly ConsumeContext<DispatchMessage> _context;

        public ConsumerDispatchContext(ConsumeContext<DispatchMessage> context, string body)
        {
            _context = context;
            _body = body;
        }

        public string Body
        {
            get { return _body; }
        }

        public Guid DispatchId
        {
            get { return _context.Message.DispatchId; }
        }

        public DateTime CreateTime
        {
            get { return _context.Message.CreateTime; }
        }

        public Uri[] Resources
        {
            get { return _context.Message.Resources; }
        }

        public string[] DispatchTypes
        {
            get { return _context.Message.PayloadType; }
        }

        public Uri Destination
        {
            get { return _context.Message.Destination; }
        }

        public bool TryGetContext<T>(out MessageDispatchContext<T> context)
            where T : class
        {
            context = null;
            return false;
        }

        public Task CompleteTask
        {
            get { return _context.CompleteTask; }
        }

        public IEnumerable<string> SupportedMessageTypes
        {
            get { return _context.SupportedMessageTypes; }
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
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

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
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

        public Headers Headers
        {
            get { return _context.Headers; }
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

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public void Respond<T>(T message)
            where T : class
        {
            _context.Respond(message);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        void ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            _context.NotifyConsumed(context, duration, consumerType);
        }

        void ConsumeContext.NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        public ConnectHandle Connect(IPublishObserver observer)
        {
            return _context.Connect(observer);
        }
    }
}