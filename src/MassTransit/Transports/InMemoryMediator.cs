namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Context.Converters;
    using GreenPipes;
    using Initializers;
    using Pipeline;
    using Pipeline.Observables;


    /// <summary>
    /// Sends messages directly to the <see cref="IReceivePipe"/>, without serialization
    /// </summary>
    public class InMemoryMediator :
        IMediator
    {
        readonly ILogContext _logContext;
        readonly IReceiveObserver _observer;
        readonly IReceivePipeDispatcher _receiver;
        readonly Uri _inputAddress;

        protected readonly SendObservable SendObservers;

        public InMemoryMediator(ILogContext logContext, IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher receiver)
        {
            _logContext = logContext;
            _observer = configuration.ReceiveObservers;
            _receiver = receiver;
            _inputAddress = configuration.InputAddress;

            SendObservers = new SendObservable();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return SendMessage(message, new MediatorPipe<T>(_inputAddress), cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendMessage(message, new MediatorPipe<T>(_inputAddress, pipe), cancellationToken);
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

            return SendMessage(message, new MediatorPipe<T>(_inputAddress, pipe), cancellationToken);
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

        protected virtual async Task SendMessage<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            LogContext.SetCurrentIfNull(_logContext);

            SendContext<T> context = new MessageSendContext<T>(message, cancellationToken);

            await pipe.Send(context).ConfigureAwait(false);

            var receiveContext = new DeliveryReceiveContext<T>(context, _observer, cancellationToken);

            try
            {
                if (SendObservers.Count > 0)
                    await SendObservers.PreSend(context).ConfigureAwait(false);

                await _receiver.Dispatch(receiveContext).ConfigureAwait(false);

                if (SendObservers.Count > 0)
                    await SendObservers.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (SendObservers.Count > 0)
                    await SendObservers.SendFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }


        readonly struct MediatorPipe<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly Uri _inputAddress;
            readonly IPipe<SendContext<T>> _pipe;

            public MediatorPipe(Uri inputAddress)
            {
                _inputAddress = inputAddress;
                _pipe = default;
            }

            public MediatorPipe(Uri inputAddress, IPipe<SendContext<T>> pipe)
            {
                _inputAddress = inputAddress;
                _pipe = pipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public async Task Send(SendContext<T> context)
            {
                context.DestinationAddress = _inputAddress;
                context.SourceAddress = _inputAddress;

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);

                if (!context.ConversationId.HasValue)
                    context.ConversationId = NewId.NextGuid();
            }
        }
    }
}
