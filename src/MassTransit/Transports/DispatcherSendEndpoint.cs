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


    public class DispatcherSendEndpoint :
        ISendEndpoint,
        IPublishEndpointProvider,
        ISendEndpointProvider
    {
        readonly IReceiveEndpointConfiguration _configuration;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ILogContext _logContext;
        readonly Uri _sourceAddress;
        readonly SendObservable _sendObservers;
        readonly DispatcherSendEndpoint _sourceEndpoint;
        readonly PublishObservable _publishObservable;

        DispatcherSendEndpoint(IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher, ILogContext logContext,
            SendObservable sendObservers)
        {
            _configuration = configuration;
            _dispatcher = dispatcher;
            _logContext = logContext;
            _sendObservers = sendObservers;

            _publishObservable = new PublishObservable();
        }

        public DispatcherSendEndpoint(IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher, ILogContext logContext,
            SendObservable sendObservers, IReceiveEndpointConfiguration sourceConfiguration, IReceivePipeDispatcher sourceDispatcher)
            : this(configuration, dispatcher, logContext, sendObservers)
        {
            _sourceAddress = sourceConfiguration.InputAddress;
            _sourceEndpoint = new DispatcherSendEndpoint(sourceConfiguration, sourceDispatcher, logContext, sendObservers);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return Task.FromResult<ISendEndpoint>(_sourceEndpoint);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return SendMessage(message, new DispatcherPipe<T>(this), cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendMessage(message, new DispatcherPipe<T>(this, pipe), cancellationToken);
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

            return SendMessage(message, new DispatcherPipe<T>(this, pipe), cancellationToken);
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

        async Task SendMessage<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            LogContext.SetCurrentIfNull(_logContext);

            SendContext<T> context = new MessageSendContext<T>(message, cancellationToken);

            await pipe.Send(context).ConfigureAwait(false);

            var receiveContext = new DispatcherReceiveContext<T>(context, this, this, _configuration.Topology.Publish, _configuration.ReceiveObservers,
                cancellationToken);

            try
            {
                if (_sendObservers.Count > 0)
                    await _sendObservers.PreSend(context).ConfigureAwait(false);

                await _dispatcher.Dispatch(receiveContext).ConfigureAwait(false);

                if (_sendObservers.Count > 0)
                    await _sendObservers.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (_sendObservers.Count > 0)
                    await _sendObservers.SendFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservable.Connect(observer);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return Task.FromResult<ISendEndpoint>(_sourceEndpoint);
        }


        readonly struct DispatcherPipe<TMessage> :
            IPipe<SendContext<TMessage>>
            where TMessage : class
        {
            readonly DispatcherSendEndpoint _endpoint;
            readonly IPipe<SendContext<TMessage>> _pipe;
            readonly ISendContextPipe _sendContextPipe;

            public DispatcherPipe(DispatcherSendEndpoint endpoint)
            {
                _endpoint = endpoint;
                _pipe = default;
                _sendContextPipe = default;
            }

            public DispatcherPipe(DispatcherSendEndpoint endpoint, IPipe<SendContext<TMessage>> pipe)
            {
                _endpoint = endpoint;
                _pipe = pipe;
                // ReSharper disable once SuspiciousTypeConversion.Global
                _sendContextPipe = pipe as ISendContextPipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public async Task Send(SendContext<TMessage> context)
            {
                context.DestinationAddress = _endpoint._configuration.InputAddress;
                context.SourceAddress = _endpoint._sourceAddress;

                if (_sendContextPipe != null)
                    await _sendContextPipe.Send(context).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);

                if (context.RequestId.HasValue)
                    context.ResponseAddress = _endpoint._sourceAddress;

                if (!context.ConversationId.HasValue)
                    context.ConversationId = NewId.NextGuid();
            }
        }
    }
}
