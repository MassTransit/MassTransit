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
namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Pipeline;


    public class HostExecuteActivityContext<TActivity, TArguments> :
        ExecuteActivityContext<TActivity, TArguments>
        where TArguments : class
        where TActivity : class, ExecuteActivity<TArguments>
    {
        readonly TActivity _activity;
        readonly ConsumeContext _consumeContext;
        readonly Execution<TArguments> _context;

        public HostExecuteActivityContext(TActivity activity, Execution<TArguments> context)
        {
            _activity = activity;
            _context = context;
            _consumeContext = _context.ConsumeContext;
        }

        public CancellationToken CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        public bool HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        public bool TryGetPayload<TPayload>(out TPayload payload) where TPayload : class
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return _context.Publish(message, cancellationToken);
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
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

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return _context.Publish<T>(values, cancellationToken);
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
        }

        public Guid? MessageId
        {
            get { return _consumeContext.MessageId; }
        }

        public Guid? RequestId
        {
            get { return _consumeContext.RequestId; }
        }

        public Guid? CorrelationId
        {
            get { return _consumeContext.CorrelationId; }
        }

        public DateTime? ExpirationTime
        {
            get { return _consumeContext.ExpirationTime; }
        }

        public Uri SourceAddress
        {
            get { return _consumeContext.SourceAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _consumeContext.DestinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _consumeContext.ResponseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _consumeContext.FaultAddress; }
        }

        public Headers Headers
        {
            get { return _consumeContext.Headers; }
        }

        public ReceiveContext ReceiveContext
        {
            get { return _consumeContext.ReceiveContext; }
        }

        public Task CompleteTask
        {
            get { return _consumeContext.CompleteTask; }
        }

        public IEnumerable<string> SupportedMessageTypes
        {
            get { return _consumeContext.SupportedMessageTypes; }
        }

        public bool HasMessageType(Type messageType)
        {
            return _consumeContext.HasMessageType(messageType);
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext) where T : class
        {
            return _consumeContext.TryGetMessage(out consumeContext);
        }

        public Task RespondAsync<T>(T message) where T : class
        {
            return _consumeContext.RespondAsync(message);
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe) where T : class
        {
            return _consumeContext.RespondAsync(message, sendPipe);
        }

        public void Respond<T>(T message) where T : class
        {
            _consumeContext.Respond(message);
        }

        public void RetryLater()
        {
            _consumeContext.RetryLater();
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _consumeContext.GetSendEndpoint(address);
        }

        public void NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            _consumeContext.NotifyConsumed(context, duration, consumerType);
        }

        public void NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            _consumeContext.NotifyFaulted(context, duration, consumerType, exception);
        }

        public Execution<TArguments> Execution
        {
            get { return _context; }
        }

        ExecuteActivity<TArguments> ExecuteActivityContext<TArguments>.Activity
        {
            get { return _activity; }
        }

        public TActivity Activity
        {
            get { return _activity; }
        }
    }
}