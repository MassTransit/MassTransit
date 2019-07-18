namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Converters;
    using Events;
    using GreenPipes;
    using GreenPipes.Payloads;
    using Initializers;
    using Pipeline.Pipes;
    using Util;


    public abstract class BaseConsumeContext :
        BasePipeContext,
        ConsumeContext
    {
        readonly Lazy<IPublishEndpoint> _publishEndpoint;
        readonly ReceiveContext _receiveContext;

        protected BaseConsumeContext(ReceiveContext receiveContext)
            : base(receiveContext)
        {
            _receiveContext = receiveContext;

            _publishEndpoint = new Lazy<IPublishEndpoint>(CreatePublishEndpoint);
        }

        protected BaseConsumeContext(ConsumeContext consumeContext)
            : base(consumeContext)
        {
            _receiveContext = consumeContext.ReceiveContext;

            _publishEndpoint = new Lazy<IPublishEndpoint>(CreatePublishEndpoint);
        }

        protected BaseConsumeContext(ConsumeContext consumeContext, IPayloadCache payloadCache)
            : base(payloadCache, consumeContext.CancellationToken)
        {
            _receiveContext = consumeContext.ReceiveContext;

            _publishEndpoint = new Lazy<IPublishEndpoint>(CreatePublishEndpoint);
        }

        public ReceiveContext ReceiveContext => _receiveContext;

        public abstract Task ConsumeCompleted { get; }

        public abstract Guid? MessageId { get; }
        public abstract Guid? RequestId { get; }
        public abstract Guid? CorrelationId { get; }
        public abstract Guid? ConversationId { get; }
        public abstract Guid? InitiatorId { get; }
        public abstract DateTime? ExpirationTime { get; }
        public abstract Uri SourceAddress { get; }
        public abstract Uri DestinationAddress { get; }
        public abstract Uri ResponseAddress { get; }
        public abstract Uri FaultAddress { get; }
        public abstract DateTime? SentTime { get; }
        public abstract Headers Headers { get; }
        public abstract HostInfo Host { get; }
        public abstract IEnumerable<string> SupportedMessageTypes { get; }
        public abstract bool HasMessageType(Type messageType);

        public abstract bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class;

        public virtual async Task RespondAsync<T>(T message)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var responsePipe = new ResponsePipe<T>(this);

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await ConsumeTask(endpoint.Send(message, responsePipe, CancellationToken)).ConfigureAwait(false);
            }
            else
                await Publish(message, responsePipe, CancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            var responsePipe = new ResponsePipe<T>(this, sendPipe);

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await ConsumeTask(endpoint.Send(message, responsePipe, CancellationToken)).ConfigureAwait(false);
            }
            else
                await Publish(message, responsePipe, CancellationToken).ConfigureAwait(false);
        }

        public virtual async Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            var responsePipe = new ResponsePipe<T>(this, sendPipe);

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await ConsumeTask(endpoint.Send(message, responsePipe, CancellationToken)).ConfigureAwait(false);
            }
            else
                await Publish(message, responsePipe, CancellationToken).ConfigureAwait(false);
        }

        public virtual Task RespondAsync(object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return RespondAsync(message, messageType);
        }

        public virtual async Task RespondAsync(object message, Type messageType)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            var responsePipe = new ResponsePipe(this);
            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await ConsumeTask(SendEndpointConverterCache.Send(endpoint, message, messageType, responsePipe, CancellationToken)).ConfigureAwait(false);
            }
            else
                await Publish(message, messageType, responsePipe, CancellationToken).ConfigureAwait(false);
        }

        public virtual Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            var messageType = message.GetType();

            return RespondAsync(message, messageType, sendPipe);
        }

        public virtual async Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            if (sendPipe == null)
                throw new ArgumentNullException(nameof(sendPipe));

            var responsePipe = new ResponsePipe(this, sendPipe);

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await ConsumeTask(SendEndpointConverterCache.Send(endpoint, message, messageType, responsePipe, CancellationToken)).ConfigureAwait(false);
            }
            else
                await Publish(message, messageType, responsePipe, CancellationToken).ConfigureAwait(false);
        }

        public virtual Task RespondAsync<T>(object values)
            where T : class
        {
            return RespondAsyncInternal(values, new ResponsePipe<T>(this));
        }

        public virtual Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return RespondAsyncInternal(values, new ResponsePipe<T>(this, sendPipe));
        }

        public virtual Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
            where T : class
        {
            return RespondAsyncInternal(values, new ResponsePipe<T>(this, sendPipe));
        }

        async Task RespondAsyncInternal<T>(object values, IPipe<SendContext<T>> responsePipe)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            IMessageInitializer<T> initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());
            var context = initializer.Create(this);

            if (ResponseAddress != null)
            {
                var endpoint = await GetSendEndpoint(ResponseAddress).ConfigureAwait(false);

                await ConsumeTask(initializer.Send(endpoint, context, values, responsePipe)).ConfigureAwait(false);
            }
            else
                await ConsumeTask(initializer.Publish(_publishEndpoint.Value, context, values, responsePipe)).ConfigureAwait(false);
        }

        public virtual void Respond<T>(T message)
            where T : class
        {
            AddConsumeTask(RespondAsync(message));
        }

        public virtual async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            var sendEndpoint = await _receiveContext.SendEndpointProvider.GetSendEndpoint(address).ConfigureAwait(false);

            return new ConsumeSendEndpoint(sendEndpoint, this, ConsumeTask);
        }

        public virtual Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            return _receiveContext.NotifyConsumed(context, duration, consumerType);
        }

        public virtual Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            AddConsumeTask(GenerateFault(context, exception));

            return _receiveContext.NotifyFaulted(context, duration, consumerType, exception);
        }

        public virtual Task Publish<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(_publishEndpoint.Value.Publish(message, cancellationToken));
        }

        public virtual Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(_publishEndpoint.Value.Publish(message, publishPipe, cancellationToken));
        }

        public virtual Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(_publishEndpoint.Value.Publish(message, publishPipe, cancellationToken));
        }

        public virtual Task Publish(object message, CancellationToken cancellationToken)
        {
            return ConsumeTask(_publishEndpoint.Value.Publish(message, cancellationToken));
        }

        public virtual Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return ConsumeTask(_publishEndpoint.Value.Publish(message, publishPipe, cancellationToken));
        }

        public virtual Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return ConsumeTask(_publishEndpoint.Value.Publish(message, messageType, cancellationToken));
        }

        public virtual Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return ConsumeTask(_publishEndpoint.Value.Publish(message, messageType, publishPipe, cancellationToken));
        }

        public virtual Task Publish<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(_publishEndpoint.Value.Publish<T>(values, cancellationToken));
        }

        public virtual Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(_publishEndpoint.Value.Publish(values, publishPipe, cancellationToken));
        }

        public virtual Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return ConsumeTask(_publishEndpoint.Value.Publish<T>(values, publishPipe, cancellationToken));
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpoint.Value.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _receiveContext.SendEndpointProvider.ConnectSendObserver(observer);
        }

        public abstract void AddConsumeTask(Task task);

        IPublishEndpoint CreatePublishEndpoint()
        {
            return _receiveContext.PublishEndpointProvider.CreatePublishEndpoint(_receiveContext.InputAddress, this);
        }

        Task ConsumeTask(Task task)
        {
            AddConsumeTask(task);

            return task;
        }

        async Task GenerateFault<T>(ConsumeContext<T> context, Exception exception)
            where T : class
        {
            Fault<T> fault = new FaultEvent<T>(context.Message, context.MessageId, HostMetadataCache.Host, exception);

            var faultPipe = new FaultPipe<T>(context);

            var destinationAddress = FaultAddress ?? ResponseAddress;
            if (destinationAddress != null)
            {
                var endpoint = await GetSendEndpoint(destinationAddress).ConfigureAwait(false);

                await ConsumeTask(endpoint.Send(fault, faultPipe, CancellationToken)).ConfigureAwait(false);
            }
            else
                await Publish(fault, faultPipe, CancellationToken).ConfigureAwait(false);
        }


        struct FaultPipe<T> :
            IPipe<SendContext<Fault<T>>>
            where T : class
        {
            readonly ConsumeContext<T> _context;

            public FaultPipe(ConsumeContext<T> context)
            {
                _context = context;
            }

            public Task Send(SendContext<Fault<T>> context)
            {
                context.TransferConsumeContextHeaders(_context);

                context.CorrelationId = _context.CorrelationId;
                context.RequestId = _context.RequestId;

                return TaskUtil.Completed;
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }
}
