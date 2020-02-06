namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context.Converters;
    using GreenPipes;


    /// <summary>
    /// The publish endpoint delivers messages to the topic/exchange/whatever based upon the publish topology of the broker, by message type.
    /// </summary>
    public class PublishEndpoint :
        IPublishEndpoint
    {
        readonly IPublishEndpointProvider _provider;

        public PublishEndpoint(IPublishEndpointProvider provider)
        {
            _provider = provider;
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return PublishInternal(cancellationToken, message);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return PublishInternal(cancellationToken, message, publishPipe);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return PublishInternal(cancellationToken, message, publishPipe);
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

            return PublishInternal<T>(cancellationToken, values);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return PublishInternal<T>(cancellationToken, values, publishPipe);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return PublishInternal<T>(cancellationToken, values, publishPipe);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _provider.ConnectPublishObserver(observer);
        }

        protected virtual async Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return await _provider.GetPublishSendEndpoint<T>().ConfigureAwait(false);
        }

        async Task PublishInternal<T>(CancellationToken cancellationToken, T message, IPipe<PublishContext<T>> pipe = default)
            where T : class
        {
            var sendEndpoint = await GetPublishSendEndpoint<T>().ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await sendEndpoint.Send(message, new PublishPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
            else
                await sendEndpoint.Send(message, cancellationToken).ConfigureAwait(false);
        }

        async Task PublishInternal<T>(CancellationToken cancellationToken, object values, IPipe<PublishContext<T>> pipe = default)
            where T : class
        {
            var sendEndpoint = await GetPublishSendEndpoint<T>().ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await sendEndpoint.Send(values, new PublishPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
            else
                await sendEndpoint.Send<T>(values, cancellationToken).ConfigureAwait(false);
        }


        readonly struct PublishPipeAdapter<T> :
            IPipe<SendContext<T>>
            where T : class
        {
            readonly IPipe<PublishContext<T>> _pipe;

            public PublishPipeAdapter(IPipe<PublishContext<T>> pipe)
            {
                _pipe = pipe;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _pipe.Probe(context);
            }

            public Task Send(SendContext<T> context)
            {
                var publishContext = context.GetPayload<PublishContext<T>>();

                return _pipe.Send(publishContext);
            }
        }
    }
}
