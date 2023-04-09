namespace MassTransit.Middleware.Outbox
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Initializers;
    using Logging;
    using Transports;
    using Util;


    public class OutboxSendEndpoint :
        ITransportSendEndpoint
    {
        readonly OutboxSendContext _context;
        readonly ITransportSendEndpoint _endpoint;

        /// <summary>
        /// Creates an send endpoint on the outbox
        /// </summary>
        /// <param name="outboxContext">The outbox context for this consume operation</param>
        /// <param name="endpoint"></param>
        public OutboxSendEndpoint(OutboxSendContext outboxContext, ISendEndpoint endpoint)
        {
            _context = outboxContext;
            _endpoint = endpoint as ITransportSendEndpoint ?? throw new ArgumentException("Must be a transport endpoint", nameof(endpoint));
        }

        public ISendEndpoint Endpoint => _endpoint;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return new EmptyConnectHandle();
        }

        public Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(pipe, _context), cancellationToken);
        }

        public async Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            SendContext<T> context =
                await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(_context), cancellationToken).ConfigureAwait(false);

            await AddSend(context).ConfigureAwait(false);
        }

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            SendContext<T> context = await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(pipe, _context), cancellationToken)
                .ConfigureAwait(false);

            await AddSend(context).ConfigureAwait(false);
        }

        public Task Send(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));

            return SendEndpointConverterCache.Send(this, message, messageType, cancellationToken);
        }

        public async Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            SendContext<T> context = await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(pipe, _context), cancellationToken)
                .ConfigureAwait(false);

            await AddSend(context).ConfigureAwait(false);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var messageType = message.GetType();

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (messageType == null)
                throw new ArgumentNullException(nameof(messageType));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendEndpointConverterCache.Send(this, message, messageType, pipe, cancellationToken);
        }

        public async Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, new OutboxSendEndpointPipe<T>(_context), cancellationToken).ConfigureAwait(false);

            SendContext<T> context =
                await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(sendPipe, _context), cancellationToken).ConfigureAwait(false);

            await AddSend(context).ConfigureAwait(false);
        }

        public async Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, new OutboxSendEndpointPipe<T>(pipe, _context), cancellationToken)
                    .ConfigureAwait(false);

            SendContext<T> context =
                await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(sendPipe, _context), cancellationToken).ConfigureAwait(false);

            await AddSend(context).ConfigureAwait(false);
        }

        public async Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, new OutboxSendEndpointPipe<T>(pipe, _context), cancellationToken)
                    .ConfigureAwait(false);

            SendContext<T> context =
                await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(sendPipe, _context), cancellationToken).ConfigureAwait(false);

            await AddSend(context).ConfigureAwait(false);
        }

        async Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            StartedActivity? activity = LogContext.Current?.StartOutboxSendActivity(context);
            StartedInstrument? instrument = LogContext.Current?.StartOutboxSendInstrument(context);
            try
            {
                await _context.AddSend(context).ConfigureAwait(false);
                activity?.Update(context);
            }
            catch (Exception ex)
            {
                activity?.AddExceptionEvent(ex);
                instrument?.AddException(ex);
                throw;
            }
            finally
            {
                activity?.Stop();
                instrument?.Stop();
            }
        }


        class OutboxSendEndpointPipe<T> :
            SendContextPipeAdapter<T>
            where T : class
        {
            readonly IServiceProvider _provider;

            public OutboxSendEndpointPipe(IServiceProvider provider)
                : base(null)
            {
                _provider = provider;
            }

            public OutboxSendEndpointPipe(IPipe<SendContext<T>> pipe, IServiceProvider provider)
                : base(pipe)
            {
                _provider = provider;
            }

            protected override void Send<TMessage>(SendContext<TMessage> context)
            {
                context.GetOrAddPayload(() => _provider);
            }

            protected override void Send(SendContext<T> context)
            {
                context.ConversationId ??= NewId.NextGuid();
            }
        }
    }
}
