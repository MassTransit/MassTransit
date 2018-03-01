// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using GreenPipes.Payloads;


    /// <summary>
    /// A consume context proxy creates a payload scope, such that anything added to the payload
    /// of the context is only added at the scope level and below.
    /// </summary>
    public abstract class ConsumeContextProxy :
        BasePipeContext,
        ConsumeContext
    {
        readonly ConsumeContext _context;

        protected ConsumeContextProxy(ConsumeContext context)
            : base(context)
        {
            _context = context;
        }

        protected ConsumeContextProxy(ConsumeContext context, IPayloadCache payloadCache)
            : base(payloadCache)
        {
            _context = context;
        }

        public virtual Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            return _context.RespondAsync(message, messageType, sendPipe);
        }

        public virtual Task RespondAsync<T>(object values) where T : class
        {
            return _context.RespondAsync<T>(values);
        }

        public virtual Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe) where T : class
        {
            return _context.RespondAsync(values, sendPipe);
        }

        public virtual Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe) where T : class
        {
            return _context.RespondAsync<T>(values, sendPipe);
        }

        public virtual Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public virtual Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe) where T : class
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public virtual Task RespondAsync(object message)
        {
            return _context.RespondAsync(message);
        }

        public virtual Task RespondAsync(object message, Type messageType)
        {
            return _context.RespondAsync(message, messageType);
        }

        public virtual Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public virtual Guid? MessageId => _context.MessageId;
        public virtual Guid? RequestId => _context.RequestId;
        public virtual Guid? CorrelationId => _context.CorrelationId;
        public virtual Guid? ConversationId => _context.ConversationId;
        public virtual Guid? InitiatorId => _context.InitiatorId;
        public virtual DateTime? ExpirationTime => _context.ExpirationTime;
        public virtual Uri SourceAddress => _context.SourceAddress;
        public virtual Uri DestinationAddress => _context.DestinationAddress;
        public virtual Uri ResponseAddress => _context.ResponseAddress;
        public virtual Uri FaultAddress => _context.FaultAddress;
        public virtual DateTime? SentTime => _context.SentTime;

        public virtual Headers Headers => _context.Headers;
        public HostInfo Host => _context.Host;

        public virtual Task Publish<T>(T message, CancellationToken cancellationToken) where T : class
        {
            return _context.Publish(message, cancellationToken);
        }

        public virtual Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken) where T : class
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public virtual Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken) where T : class
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public virtual Task Publish(object message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        public virtual Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public virtual Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, cancellationToken);
        }

        public virtual Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        public virtual Task Publish<T>(object values, CancellationToken cancellationToken) where T : class
        {
            return _context.Publish<T>(values, cancellationToken);
        }

        public virtual Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe,
            CancellationToken cancellationToken) where T : class
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        public virtual Task Publish<T>(object values, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken) where T : class
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
        }

        public ReceiveContext ReceiveContext => _context.ReceiveContext;
        public Task CompleteTask => _context.CompleteTask;
        public virtual IEnumerable<string> SupportedMessageTypes => _context.SupportedMessageTypes;

        public virtual bool HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        public virtual bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            ConsumeContext<T> messageContext;
            if (_context.TryGetMessage(out messageContext))
            {
                consumeContext = new MessageConsumeContext<T>(this, messageContext.Message);
                return true;
            }

            consumeContext = null;
            return false;
        }

        public virtual Task RespondAsync<T>(T message) where T : class
        {
            return _context.RespondAsync(message);
        }

        public virtual void Respond<T>(T message) where T : class
        {
            _context.Respond(message);
        }

        public virtual Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        public virtual Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        public virtual Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        public virtual ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public virtual ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }
    }


    /// <summary>
    /// A consume context proxy creates a payload scope, such that anything added to the payload
    /// of the context is only added at the scope level and below.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class ConsumeContextProxy<TMessage> :
        ConsumeContextProxy,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public ConsumeContextProxy(ConsumeContext<TMessage> context)
            : base(context)
        {
            _context = context;
        }

        public ConsumeContextProxy(ConsumeContext<TMessage> context, IPayloadCache payloadCache)
            : base(context, payloadCache)
        {
            _context = context;
        }

        public TMessage Message => _context.Message;

        public virtual Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return base.NotifyConsumed(this, duration, consumerType);
        }

        public virtual Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return base.NotifyFaulted(this, duration, consumerType, exception);
        }
    }
}