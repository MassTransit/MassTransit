namespace MassTransit.Courier
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using Initializers;
    using Observables;
    using Serialization;


    public class RoutingSlipBuilderSendEndpoint :
        ISendEndpoint
    {
        readonly string _activityName;
        readonly IRoutingSlipSendEndpointTarget _builder;
        readonly Uri _destinationAddress;
        readonly RoutingSlipEvents _events;
        readonly RoutingSlipEventContents _include;
        readonly SendObservable _observers;

        public RoutingSlipBuilderSendEndpoint(IRoutingSlipSendEndpointTarget builder, Uri destinationAddress, RoutingSlipEvents events, string activityName,
            RoutingSlipEventContents include = RoutingSlipEventContents.All)
        {
            _observers = new SendObservable();
            _builder = builder;
            _events = events;
            _activityName = activityName;
            _include = include;
            _destinationAddress = destinationAddress;
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return Send(message, Pipe.Empty<SendContext<T>>(), cancellationToken);
        }

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var context = new RoutingSlipSendContext<T>(message, cancellationToken, _destinationAddress);

            await pipe.Send(context).ConfigureAwait(false);

            _builder.AddSubscription(_destinationAddress, _events, _include, _activityName, context.GetMessageEnvelope());
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return Send(message, (IPipe<SendContext<T>>)pipe, cancellationToken);
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
                await MessageInitializerCache<T>.InitializeMessage(values, cancellationToken).ConfigureAwait(false);

            await Send(message, sendPipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, pipe, cancellationToken).ConfigureAwait(false);

            await Send(message, sendPipe, cancellationToken).ConfigureAwait(false);
        }

        public async Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            (var message, IPipe<SendContext<T>> sendPipe) =
                await MessageInitializerCache<T>.InitializeMessage(values, pipe, cancellationToken).ConfigureAwait(false);

            await Send(message, sendPipe, cancellationToken).ConfigureAwait(false);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }


        class RoutingSlipSendContext<T> :
            MessageSendContext<T>
            where T : class
        {
            public RoutingSlipSendContext(T message, CancellationToken cancellationToken, Uri destinationAddress)
                : base(message, cancellationToken)
            {
                DestinationAddress = destinationAddress;

                Serializer = SystemTextJsonMessageSerializer.Instance;
            }

            public MessageEnvelope GetMessageEnvelope()
            {
                var envelope = new JsonMessageEnvelope(this, Message, MessageTypeCache<T>.MessageTypeNames);

                return envelope;
            }
        }
    }
}
