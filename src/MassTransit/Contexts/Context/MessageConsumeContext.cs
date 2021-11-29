namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Initializers;


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

        public bool HasPayloadType(Type payloadType)
        {
            return payloadType.GetTypeInfo().IsInstanceOfType(this) || _context.HasPayloadType(payloadType);
        }

        public bool TryGetPayload<T>(out T payload)
            where T : class
        {
            if (this is T context)
            {
                payload = context;
                return true;
            }

            return _context.TryGetPayload(out payload);
        }

        public T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
            where T : class
        {
            if (this is T context)
                return context;

            return _context.GetOrAddPayload(payloadFactory);
        }

        public T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
            where T : class
        {
            if (this is T context)
                return context;

            return _context.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public CancellationToken CancellationToken => _context.CancellationToken;

        public Guid? MessageId => _context.MessageId;

        public Guid? RequestId => _context.RequestId;

        public Guid? CorrelationId => _context.CorrelationId;

        public Guid? ConversationId => _context.ConversationId;

        public Guid? InitiatorId => _context.InitiatorId;

        public DateTime? ExpirationTime => _context.ExpirationTime;

        public Uri SourceAddress => _context.SourceAddress;

        public Uri DestinationAddress => _context.DestinationAddress;

        public Uri ResponseAddress => _context.ResponseAddress;

        public Uri FaultAddress => _context.FaultAddress;

        public DateTime? SentTime => _context.SentTime;

        public Headers Headers => _context.Headers;

        public HostInfo Host => _context.Host;

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _context.ConnectPublishObserver(observer);
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            return _context.Publish(message, cancellationToken);
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish(object message, CancellationToken cancellationToken)
        {
            return _context.Publish(message, cancellationToken);
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, cancellationToken);
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _context.Publish(message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            return _context.Publish<T>(values, cancellationToken);
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return _context.Publish(values, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return _context.Publish<T>(values, publishPipe, cancellationToken);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _context.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.GetSendEndpoint(address);
        }

        public ReceiveContext ReceiveContext => _context.ReceiveContext;
        public SerializerContext SerializerContext => _context.SerializerContext;

        public Task ConsumeCompleted => _context.ConsumeCompleted;

        public IEnumerable<string> SupportedMessageTypes => _context.SupportedMessageTypes;

        public bool HasMessageType(Type messageType)
        {
            return _context.HasMessageType(messageType);
        }

        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            return _context.TryGetMessage(out consumeContext);
        }

        public void AddConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);
        }

        public Task RespondAsync<T>(T message)
            where T : class
        {
            return _context.RespondAsync(message);
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public Task RespondAsync(object message)
        {
            return _context.RespondAsync(message);
        }

        public Task RespondAsync(object message, Type messageType)
        {
            return _context.RespondAsync(message, messageType);
        }

        public Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            return _context.RespondAsync(message, sendPipe);
        }

        public Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            return _context.RespondAsync(message, messageType, sendPipe);
        }

        public Task RespondAsync<T>(object values)
            where T : class
        {
            return ResponseAsyncWithMessage<T>(values);
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return ResponseAsyncWithMessage(values, sendPipe);
        }

        public Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            return ResponseAsyncWithMessage<T>(values, sendPipe);
        }

        public void Respond<T>(T message)
            where T : class
        {
            _context.Respond(message);
        }

        public Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            return _context.NotifyConsumed(context, duration, consumerType);
        }

        public Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            return _context.NotifyFaulted(context, duration, consumerType, exception);
        }

        /// <summary>
        /// Initializes the response with the request message, and then uses the initializer to initialize the
        /// remaining properties using the <paramref name="values" /> parameter.
        /// </summary>
        async Task ResponseAsyncWithMessage<T>(object values, IPipe<SendContext<T>> responsePipe = default)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var responseEndpoint = await this.GetResponseEndpoint<T>().ConfigureAwait(false);

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(_context, values, new object[] { Message }, responsePipe).ConfigureAwait(false);

            await ConsumeTask(responseEndpoint.Send(message, sendPipe, _context.CancellationToken)).ConfigureAwait(false);
        }

        Task ConsumeTask(Task task)
        {
            _context.AddConsumeTask(task);

            return task;
        }
    }
}
