namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context.Converters;
    using GreenPipes;
    using Initializers;
    using Pipeline;


    public class SendEndpoint :
        ISendEndpoint,
        IAsyncDisposable
    {
        readonly ISendPipe _sendPipe;
        readonly ConnectHandle _observerHandle;
        readonly ISendTransport _transport;

        public SendEndpoint(ISendTransport transport, IMessageSerializer serializer, Uri destinationAddress, Uri sourceAddress, ISendPipe sendPipe,
            ConnectHandle observerHandle = null)
        {
            _transport = transport;
            _sendPipe = sendPipe;
            _observerHandle = observerHandle;

            Serializer = serializer;
            DestinationAddress = destinationAddress;
            SourceAddress = sourceAddress;
        }

        IMessageSerializer Serializer { get; }

        Uri DestinationAddress { get; }

        Uri SourceAddress { get; }

        public async Task DisposeAsync(CancellationToken cancellationToken)
        {
            _observerHandle?.Disconnect();

            switch (_transport)
            {
                case IAsyncDisposable disposable:
                    await disposable.DisposeAsync(cancellationToken).ConfigureAwait(false);
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _transport.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var settingsPipe = new EndpointSendContextPipe<T>(this);

            return _transport.Send(message, settingsPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var settingsPipe = new EndpointSendContextPipe<T>(this, pipe);

            return _transport.Send(message, settingsPipe, cancellationToken);
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

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return MessageInitializerCache<T>.Send(this, values, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var settingsPipe = new EndpointSendContextPipe<T>(this, pipe);

            return _transport.Send(message, settingsPipe, cancellationToken);
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

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return MessageInitializerCache<T>.Send(this, values, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return MessageInitializerCache<T>.Send(this, values, pipe, cancellationToken);
        }


        class EndpointSendContextPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly SendEndpoint _endpoint;
            readonly IPipe<SendContext<T>> _pipe;

            public EndpointSendContextPipe(SendEndpoint endpoint)
            {
                _endpoint = endpoint;
            }

            public EndpointSendContextPipe(SendEndpoint endpoint, IPipe<SendContext<T>> pipe)
            {
                _endpoint = endpoint;
                _pipe = pipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public async Task Send(SendContext<T> context)
            {
                context.Serializer = _endpoint.Serializer;
                context.DestinationAddress = _endpoint.DestinationAddress;

                if (context.SourceAddress == null)
                    context.SourceAddress = _endpoint.SourceAddress;

                if (_pipe is ISendContextPipe sendContextPipe)
                    await sendContextPipe.Send(context).ConfigureAwait(false);

                if (_endpoint._sendPipe != null)
                    await _endpoint._sendPipe.Send(context).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);

                if (!context.ConversationId.HasValue)
                    context.ConversationId = NewId.NextGuid();
            }
        }
    }
}
