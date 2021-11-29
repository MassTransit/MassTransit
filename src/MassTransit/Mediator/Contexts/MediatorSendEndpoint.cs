namespace MassTransit.Mediator.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Initializers;
    using Logging;
    using Observables;
    using Serialization;
    using Transports;


    public class MediatorSendEndpoint :
        ISendEndpoint,
        IPublishEndpointProvider,
        ISendEndpointProvider
    {
        readonly Uri _destinationAddress;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ILogContext _logContext;
        readonly IObjectDeserializer _objectDeserializer;
        readonly MediatorPublishSendEndpoint _publishSendEndpoint;
        readonly IPublishTopologyConfigurator _publishTopology;
        readonly ReceiveObservable _receiveObservers;
        readonly SendObservable _sendObservers;
        readonly ISendPipe _sendPipe;
        readonly Uri _sourceAddress;
        readonly MediatorSendEndpoint _sourceEndpoint;

        MediatorSendEndpoint(IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher, ILogContext logContext,
            SendObservable sendObservers)
        {
            _dispatcher = dispatcher;
            _logContext = logContext;
            _sendObservers = sendObservers;

            _destinationAddress = configuration.InputAddress;
            _publishTopology = configuration.Topology.Publish;
            _receiveObservers = configuration.ReceiveObservers;

            _objectDeserializer = SystemTextJsonMessageSerializer.Instance;

            _sendPipe = configuration.Send.CreatePipe();
            _publishSendEndpoint = new MediatorPublishSendEndpoint(this, configuration.Publish.CreatePipe());
        }

        public MediatorSendEndpoint(IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher, ILogContext logContext,
            SendObservable sendObservers, IReceiveEndpointConfiguration sourceConfiguration, IReceivePipeDispatcher sourceDispatcher)
            : this(configuration, dispatcher, logContext, sendObservers)
        {
            _sourceAddress = sourceConfiguration.InputAddress;
            _sourceEndpoint = new MediatorSendEndpoint(sourceConfiguration, sourceDispatcher, logContext, sendObservers);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishSendEndpoint.ConnectPublishObserver(observer);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return Task.FromResult<ISendEndpoint>(_publishSendEndpoint);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return SendMessage(message, new MediatorPipe<T>(this), cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendMessage(message, new MediatorPipe<T>(this, pipe), cancellationToken);
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

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return SendMessage(message, new MediatorPipe<T>(this, pipe), cancellationToken);
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
                await MessageInitializerCache<T>.InitializeMessage(values, new MediatorPipe<T>(this), cancellationToken).ConfigureAwait(false);

            await SendMessage(message, sendPipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, new MediatorPipe<T>(this, pipe), cancellationToken).ConfigureAwait(false);

            await SendMessage(message, sendPipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, new MediatorPipe<T>(this, pipe), cancellationToken).ConfigureAwait(false);

            await SendMessage(message, sendPipe, cancellationToken).ConfigureAwait(false);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return Task.FromResult<ISendEndpoint>(address.Equals(_sourceAddress) ? _sourceEndpoint : this);
        }

        async Task SendMessage<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            LogContext.SetCurrentIfNull(_logContext);

            var context = new MessageSendContext<T>(message, cancellationToken);

            await pipe.Send(context).ConfigureAwait(false);

            var receiveContext = new MediatorReceiveContext<T>(context, this, this, _publishTopology, _receiveObservers, _objectDeserializer, cancellationToken)
            {
                IsDelivered = context.IsPublish && !context.Mandatory
            };

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


        class MediatorPipe<TMessage> :
            IPipe<SendContext<TMessage>>
            where TMessage : class
        {
            readonly MediatorSendEndpoint _endpoint;
            readonly IPipe<SendContext<TMessage>> _pipe;

            public MediatorPipe(MediatorSendEndpoint endpoint)
            {
                _endpoint = endpoint;
                _pipe = default;
            }

            public MediatorPipe(MediatorSendEndpoint endpoint, IPipe<SendContext<TMessage>> pipe)
            {
                _endpoint = endpoint;
                _pipe = pipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public async Task Send(SendContext<TMessage> context)
            {
                context.DestinationAddress = _endpoint._destinationAddress;

                context.SourceAddress ??= _endpoint._sourceAddress;

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (_pipe is ISendContextPipe sendContextPipe)
                    await sendContextPipe.Send(context).ConfigureAwait(false);

                await _endpoint._sendPipe.Send(context).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);

                context.ConversationId ??= NewId.NextGuid();
            }
        }
    }
}
