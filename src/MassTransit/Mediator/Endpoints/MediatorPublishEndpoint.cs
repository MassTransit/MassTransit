namespace MassTransit.Mediator.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using Context.Converters;
    using Contexts;
    using GreenPipes;
    using Initializers;
    using Pipeline;
    using Pipeline.Observables;
    using Topology;
    using Transports;


    public class MediatorPublishEndpoint :
        ISendEndpoint,
        IPublishObserverConnector
    {
        readonly MediatorSendEndpoint _sendEndpoint;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly ILogContext _logContext;
        readonly SendObservable _sendObservers;
        readonly PublishObservable _observers;
        readonly IPublishPipe _publishPipe;
        readonly Uri _sourceAddress;
        readonly Uri _destinationAddress;
        readonly IPublishTopologyConfigurator _publishTopology;
        readonly ReceiveObservable _receiveObservers;

        MediatorPublishEndpoint(IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher, ILogContext logContext,
            SendObservable sendObservers)
        {
            _dispatcher = dispatcher;
            _logContext = logContext;
            _sendObservers = sendObservers;

            _destinationAddress = configuration.InputAddress;
            _publishTopology = configuration.Topology.Publish;
            _receiveObservers = configuration.ReceiveObservers;

            _publishPipe = configuration.Publish.CreatePipe();

            _observers = new PublishObservable();
        }

        public MediatorPublishEndpoint(MediatorSendEndpoint sendEndpoint, IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher,
            ILogContext logContext, SendObservable sendObservers, IReceiveEndpointConfiguration sourceConfiguration)
            : this(configuration, dispatcher, logContext, sendObservers)
        {
            _sourceAddress = sourceConfiguration.InputAddress;
            _sendEndpoint = sendEndpoint;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _observers.Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpoint.ConnectSendObserver(observer);
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

            var receiveContext =
                new MediatorReceiveContext<T>(context, _sendEndpoint, _sendEndpoint, _publishTopology, _receiveObservers, cancellationToken)
                {
                    IsDelivered = true
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
            readonly MediatorPublishEndpoint _endpoint;
            readonly IPipe<SendContext<TMessage>> _pipe;

            public MediatorPipe(MediatorPublishEndpoint endpoint)
            {
                _endpoint = endpoint;
                _pipe = default;
            }

            public MediatorPipe(MediatorPublishEndpoint endpoint, IPipe<SendContext<TMessage>> pipe)
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
                context.DestinationAddress ??= _endpoint._destinationAddress;

                context.SourceAddress ??= _endpoint._sourceAddress;

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (_pipe is ISendContextPipe sendContextPipe)
                    await sendContextPipe.Send(context).ConfigureAwait(false);

                var publishContext = context.GetPayload<PublishContext<TMessage>>();

                await _endpoint._publishPipe.Send(publishContext).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);

                context.ConversationId ??= NewId.NextGuid();
            }
        }
    }
}
