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
    /// The publish endpoint delivers messages to the topic/exchange/whatever based upon the publish topology of the broker, by message type.
    /// </summary>
    public class PublishEndpoint :
        IPublishEndpoint
    {
        readonly ConsumeContext _consumeContext;
        readonly IPublishEndpointProvider _endpointProvider;
        readonly PublishObservable _publishObserver;
        readonly IPublishPipe _publishPipe;
        readonly Uri _sourceAddress;

        public PublishEndpoint(Uri sourceAddress, IPublishEndpointProvider endpointProvider, PublishObservable publishObserver, IPublishPipe publishPipe,
            ConsumeContext consumeContext)
        {
            _sourceAddress = sourceAddress;
            _endpointProvider = endpointProvider;
            _publishObserver = publishObserver;
            _publishPipe = publishPipe;
            _consumeContext = consumeContext;
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            var adapter = new PublishEndpointPipeAdapter<T>(message, _publishPipe, _publishObserver, _sourceAddress, _consumeContext);

            return Publish(cancellationToken, message, adapter);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            var adapter = new PublishEndpointPipeAdapter<T>(message, publishPipe, _publishPipe, _publishObserver, _sourceAddress, _consumeContext);

            return Publish(cancellationToken, message, adapter);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            var adapter = new PublishEndpointPipeAdapter<T>(message, publishPipe, _publishPipe, _publishObserver, _sourceAddress, _consumeContext);

            return Publish(cancellationToken, message, adapter);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            if (_consumeContext != null)
                return initializer.Publish(this, initializer.Create(_consumeContext), values);

            return initializer.Publish(this, values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            if (_consumeContext != null)
                return initializer.Publish(this, initializer.Create(_consumeContext), values, publishPipe);

            return initializer.Publish(this, values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            var initializer = MessageInitializerCache<T>.GetInitializer(values.GetType());

            if (_consumeContext != null)
                return initializer.Publish(this, initializer.Create(_consumeContext), values, publishPipe);

            return initializer.Publish(this, values, publishPipe, cancellationToken);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _endpointProvider.ConnectPublishObserver(observer);
        }

        async Task Publish<T>(CancellationToken cancellationToken, T message, PublishEndpointPipeAdapter<T> adapter)
            where T : class
        {
            try
            {
                var sendEndpoint = await _endpointProvider.GetPublishSendEndpoint(message).ConfigureAwait(false);

                await sendEndpoint.Send(message, adapter, cancellationToken).ConfigureAwait(false);

                if (adapter.ObserverCount > 0)
                    await adapter.PostPublish().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (adapter.ObserverCount > 0)
                    await adapter.PublishFaulted(ex).ConfigureAwait(false);

                throw;
            }
        }
    }
}
