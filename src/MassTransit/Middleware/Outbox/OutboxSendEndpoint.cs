namespace MassTransit.Middleware.Outbox
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Initializers;
    using Transports;
    using Util;


    public class OutboxSendEndpoint :
        ITransportSendEndpoint
    {
        readonly ITransportSendEndpoint _endpoint;
        readonly OutboxSendContext _outboxContext;

        public ISendEndpoint Endpoint => _endpoint;

        /// <summary>
        /// Creates an send endpoint on the outbox
        /// </summary>
        /// <param name="outboxContext">The outbox context for this consume operation</param>
        /// <param name="endpoint"></param>
        public OutboxSendEndpoint(OutboxSendContext outboxContext, ISendEndpoint endpoint)
        {
            _outboxContext = outboxContext;
            _endpoint = endpoint as ITransportSendEndpoint ?? throw new ArgumentException("Must be a transport endpoint", nameof(endpoint));
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return new EmptyConnectHandle();
        }

        public Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(pipe), cancellationToken);
        }

        public async Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            SendContext<T> context = await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(), cancellationToken).ConfigureAwait(false);

            await _outboxContext.AddSend(context).ConfigureAwait(false);
        }

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            SendContext<T> context = await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(pipe), cancellationToken).ConfigureAwait(false);

            await _outboxContext.AddSend(context).ConfigureAwait(false);
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

            SendContext<T> context = await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(pipe), cancellationToken).ConfigureAwait(false);

            await _outboxContext.AddSend(context).ConfigureAwait(false);
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
                await MessageInitializerCache<T>.InitializeMessage(values, new OutboxSendEndpointPipe<T>(), cancellationToken).ConfigureAwait(false);

            SendContext<T> context =
                await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(sendPipe), cancellationToken).ConfigureAwait(false);

            await _outboxContext.AddSend(context).ConfigureAwait(false);
        }

        public async Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, new OutboxSendEndpointPipe<T>(pipe), cancellationToken).ConfigureAwait(false);

            SendContext<T> context =
                await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(sendPipe), cancellationToken).ConfigureAwait(false);

            await _outboxContext.AddSend(context).ConfigureAwait(false);
        }

        public async Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, new OutboxSendEndpointPipe<T>(pipe), cancellationToken).ConfigureAwait(false);

            SendContext<T> context =
                await _endpoint.CreateSendContext(message, new OutboxSendEndpointPipe<T>(sendPipe), cancellationToken).ConfigureAwait(false);

            await _outboxContext.AddSend(context).ConfigureAwait(false);
        }


        class OutboxSendEndpointPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly IPipe<SendContext<T>> _pipe;
            readonly ISendContextPipe _sendContextPipe;

            public OutboxSendEndpointPipe()
            {
                _pipe = default;
                _sendContextPipe = default;
            }

            public OutboxSendEndpointPipe(IPipe<SendContext<T>> pipe)
            {
                _pipe = pipe;
                _sendContextPipe = pipe as ISendContextPipe;
            }

            public void Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public async Task Send(SendContext<T> context)
            {
                if (_sendContextPipe != null)
                    await _sendContextPipe.Send(context).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);

                context.ConversationId ??= NewId.NextGuid();
            }
        }
    }
}
