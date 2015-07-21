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
    using Util;


    public class RetryConsumeContext :
        ConsumeContext
    {
        readonly ConsumeContext _context;
        readonly IList<PendingFault> _pendingFaults;

        public RetryConsumeContext(ConsumeContext context)
        {
            _context = context;
            _pendingFaults = new List<PendingFault>();
        }

        Task ConsumeContext.CompleteTask
        {
            get { return _context.CompleteTask; }
        }

        IEnumerable<string> ConsumeContext.SupportedMessageTypes
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

        bool PipeContext.HasPayloadType(Type contextType)
        {
            return _context.HasPayloadType(contextType);
        }

        bool PipeContext.TryGetPayload<TPayload>(out TPayload payload)
        {
            return _context.TryGetPayload(out payload);
        }

        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory) where TPayload : class
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        Guid? MessageContext.MessageId
        {
            get { return _context.MessageId; }
        }

        Guid? MessageContext.RequestId
        {
            get { return _context.RequestId; }
        }

        Guid? MessageContext.CorrelationId
        {
            get { return _context.CorrelationId; }
        }

        DateTime? MessageContext.ExpirationTime
        {
            get { return _context.ExpirationTime; }
        }

        Uri MessageContext.SourceAddress
        {
            get { return _context.SourceAddress; }
        }

        Uri MessageContext.DestinationAddress
        {
            get { return _context.DestinationAddress; }
        }

        Uri MessageContext.ResponseAddress
        {
            get { return _context.ResponseAddress; }
        }

        Uri MessageContext.FaultAddress
        {
            get { return _context.FaultAddress; }
        }

        Headers MessageContext.Headers
        {
            get { return _context.Headers; }
        }

        CancellationToken PipeContext.CancellationToken
        {
            get { return _context.CancellationToken; }
        }

        ReceiveContext ConsumeContext.ReceiveContext
        {
            get { return _context.ReceiveContext; }
        }

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

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
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

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            _pendingFaults.Add(new PendingFault<T>(context, duration, consumerType, exception));

            return TaskUtil.Completed;
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext) where T : class
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

        public void ClearPendingFaults()
        {
            _pendingFaults.Clear();
        }

        public void NotifyPendingFaults()
        {
            foreach (PendingFault pendingFault in _pendingFaults)
                pendingFault.Notify(_context);
        }


        interface PendingFault
        {
            void Notify(ConsumeContext context);
        }


        class PendingFault<T> :
            PendingFault
            where T : class
        {
            readonly string _consumerType;
            readonly ConsumeContext<T> _context;
            readonly TimeSpan _elapsed;
            readonly Exception _exception;

            public PendingFault(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
            {
                _context = context;
                _elapsed = elapsed;
                _consumerType = consumerType;
                _exception = exception;
            }

            public void Notify(ConsumeContext context)
            {
                context.NotifyFaulted(_context, _elapsed, _consumerType, _exception);
            }
        }
    }


    public class RetryConsumeContext<T> :
        RetryConsumeContext,
        ConsumeContext<T>
        where T : class
    {
        readonly ConsumeContext<T> _context;

        public RetryConsumeContext(ConsumeContext<T> context)
            : base(context)
        {
            _context = context;
        }

        T ConsumeContext<T>.Message => _context.Message;

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return NotifyFaulted(_context, duration, consumerType, exception);
        }
    }
}