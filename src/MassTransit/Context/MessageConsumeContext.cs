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
    using GreenPipes;
    using Initializers;
    using Pipeline.Pipes;


    public class MessageConsumeContext<TMessage> :
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly ConsumeContext _context;

        public MessageConsumeContext(ConsumeContext context, TMessage message)
        {
            _context = context;

            Message = message;
        }

        public TMessage Message { get; }

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(this, duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(this, duration, consumerType, exception);
        }

        bool PipeContext.HasPayloadType(Type payloadType)
        {
            return _context.HasPayloadType(payloadType);
        }

        bool PipeContext.TryGetPayload<T>(out T payload)
        {
            return _context.TryGetPayload(out payload);
        }

        T PipeContext.GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
        {
            return _context.GetOrAddPayload(payloadFactory);
        }

        T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        CancellationToken PipeContext.CancellationToken => _context.CancellationToken;

        Guid? MessageContext.MessageId => _context.MessageId;

        Guid? MessageContext.RequestId => _context.RequestId;

        Guid? MessageContext.CorrelationId => _context.CorrelationId;

        Guid? MessageContext.ConversationId => _context.ConversationId;

        Guid? MessageContext.InitiatorId => _context.InitiatorId;

        DateTime? MessageContext.ExpirationTime => _context.ExpirationTime;

        Uri MessageContext.SourceAddress => _context.SourceAddress;

        Uri MessageContext.DestinationAddress => _context.DestinationAddress;

        Uri MessageContext.ResponseAddress => _context.ResponseAddress;

        Uri MessageContext.FaultAddress => _context.FaultAddress;

        DateTime? MessageContext.SentTime => _context.SentTime;

        Headers MessageContext.Headers => _context.Headers;

        HostInfo MessageContext.Host => _context.Host;

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
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

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
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

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        ReceiveContext ConsumeContext.ReceiveContext => _context.ReceiveContext;

        Task ConsumeContext.ConsumeCompleted => _context.ConsumeCompleted;

        IEnumerable<string> ConsumeContext.SupportedMessageTypes => _context.SupportedMessageTypes;

        bool ConsumeContext.HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        bool ConsumeContext.TryGetMessage<T>(out ConsumeContext<T> consumeContext)
        {
            return _context.TryGetMessage(out consumeContext);
        }

        void ConsumeContext.AddConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);
        }

        Task ConsumeContext.RespondAsync<T>(T message)
        {
            return _context.RespondAsync(message);
        }

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        Task ConsumeContext.RespondAsync(object message)
        {
            return _context.RespondAsync(message);
        }

        Task ConsumeContext.RespondAsync(object message, Type messageType)
        {
            return _context.RespondAsync(message, messageType);
        }

        Task ConsumeContext.RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        Task ConsumeContext.RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            return _context.RespondAsync(message, messageType, sendPipe);
        }

        Task ConsumeContext.RespondAsync<T>(object values)
        {
            return RespondAsyncInternal<T>(values, new ResponsePipe<T>(this));
        }

        Task ConsumeContext.RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
        {
            return RespondAsyncInternal<T>(values, new ResponsePipe<T>(this, sendPipe));
        }

        Task ConsumeContext.RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
        {
            return RespondAsyncInternal<T>(values, new ResponsePipe<T>(this, sendPipe));
        }

        void ConsumeContext.Respond<T>(T message)
        {
            _context.Respond(message);
        }

        Task ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        Task ConsumeContext.NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        async Task RespondAsyncInternal<T>(object values, IPipe<SendContext<T>> responsePipe)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var context = await MessageInitializerCache<T>.Initialize(Message, _context.CancellationToken).ConfigureAwait(false);

            IMessageInitializer<T> initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            if (_context.ResponseAddress != null)
            {
                var endpoint = await _context.GetSendEndpoint(_context.ResponseAddress).ConfigureAwait(false);

                await ConsumeTask(initializer.Send(endpoint, context, values, responsePipe)).ConfigureAwait(false);
            }
            else
                await ConsumeTask(initializer.Publish(_context, context, values, responsePipe)).ConfigureAwait(false);
        }

        Task ConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);

            return task;
        }
    }
}
