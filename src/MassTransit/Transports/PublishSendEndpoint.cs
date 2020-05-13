namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context.Converters;
    using GreenPipes;
    using Initializers;
    using Pipeline;
    using Pipeline.Observables;


    /// <summary>
    /// Intercepts the ISendEndpoint and makes it part of the current consume context
    /// </summary>
    public class PublishSendEndpoint :
        ISendEndpoint,
        IPublishObserverConnector
    {
        readonly ISendEndpoint _endpoint;
        readonly IPublishPipe _publishPipe;
        readonly PublishObservable _observers;

        public PublishSendEndpoint(ISendEndpoint endpoint, IPublishPipe publishPipe)
        {
            _endpoint = endpoint;
            _publishPipe = publishPipe;

            _observers = new PublishObservable();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var sendContextPipe = new PublishPipeAdapter<T>(_publishPipe);

            return _endpoint.Send(message, sendContextPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var sendContextPipe = new PublishPipeAdapter<T>(_publishPipe, pipe);

            return _endpoint.Send(message, sendContextPipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var sendContextPipe = new PublishPipeAdapter<T>(_publishPipe, pipe);

            return _endpoint.Send(message, sendContextPipe, cancellationToken);
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

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return MessageInitializerCache<T>.Send(this, values, new PublishPipeAdapter<T>(_publishPipe), cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return MessageInitializerCache<T>.Send(this, values, new PublishPipeAdapter<T>(_publishPipe, pipe), cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return MessageInitializerCache<T>.Send(this, values, new PublishPipeAdapter<T>(_publishPipe, pipe), cancellationToken);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _observers.Connect(observer);
        }


        class PublishPipeAdapter<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly IPublishPipe _publishPipe;
            readonly IPipe<SendContext<T>> _pipe;

            public PublishPipeAdapter(IPublishPipe publishPipe, IPipe<SendContext<T>> pipe)
            {
                _publishPipe = publishPipe;
                _pipe = pipe;
            }

            public PublishPipeAdapter(IPublishPipe publishPipe)
            {
                _publishPipe = publishPipe;
                _pipe = default;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe.Probe(context);
            }

            public async Task Send(SendContext<T> context)
            {
                var publishContext = context.GetPayload<PublishContext<T>>();

                await _publishPipe.Send(publishContext).ConfigureAwait(false);

                if (_pipe.IsNotEmpty())
                    await _pipe.Send(context).ConfigureAwait(false);
            }
        }
    }
}
