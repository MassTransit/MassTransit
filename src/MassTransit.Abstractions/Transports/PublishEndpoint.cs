namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;


    /// <summary>
    /// The publish endpoint delivers messages to the topic/exchange/whatever based upon the publish topology of the broker, by message type.
    /// </summary>
    public class PublishEndpoint :
        IPublishEndpoint
    {
        public PublishEndpoint(IPublishEndpointProvider provider)
        {
            PublishEndpointProvider = provider;
        }

        protected IPublishEndpointProvider PublishEndpointProvider { get; set; }

        public Task Publish<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            return PublishInternal(cancellationToken, message);
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return PublishInternal(cancellationToken, message, publishPipe);
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return PublishInternal(cancellationToken, message, publishPipe);
        }

        public Task Publish(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return PublishInternal<T>(cancellationToken, values);
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return PublishInternal(cancellationToken, values, publishPipe);
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return PublishInternal<T>(cancellationToken, values, publishPipe);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return PublishEndpointProvider.ConnectPublishObserver(observer);
        }

        protected virtual Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return PublishEndpointProvider.GetPublishSendEndpoint<T>();
        }

        Task PublishInternal<T>(CancellationToken cancellationToken, T message, IPipe<PublishContext<T>>? pipe = default)
            where T : class
        {
            Task<ISendEndpoint> sendEndpointTask = GetPublishSendEndpoint<T>();
            if (sendEndpointTask.Status == TaskStatus.RanToCompletion)
            {
                var sendEndpoint = sendEndpointTask.Result;

                return pipe != null && pipe.IsNotEmpty()
                    ? sendEndpoint.Send(message, new PublishSendPipeAdapter<T>(pipe), cancellationToken)
                    : sendEndpoint.Send(message, cancellationToken);
            }

            async Task PublishAsync()
            {
                var sendEndpoint = await sendEndpointTask.ConfigureAwait(false);

                if (pipe != null && pipe.IsNotEmpty())
                    await sendEndpoint.Send(message, new PublishSendPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
                else
                    await sendEndpoint.Send(message, cancellationToken).ConfigureAwait(false);
            }

            return PublishAsync();
        }

        Task PublishInternal<T>(CancellationToken cancellationToken, object values, IPipe<PublishContext<T>>? pipe = default)
            where T : class
        {
            Task<ISendEndpoint> sendEndpointTask = GetPublishSendEndpoint<T>();
            if (sendEndpointTask.Status == TaskStatus.RanToCompletion)
            {
                var sendEndpoint = sendEndpointTask.Result;

                return pipe != null && pipe.IsNotEmpty()
                    ? sendEndpoint.Send(values, new PublishSendPipeAdapter<T>(pipe), cancellationToken)
                    : sendEndpoint.Send<T>(values, cancellationToken);
            }

            async Task PublishAsync()
            {
                var sendEndpoint = await sendEndpointTask.ConfigureAwait(false);

                if (pipe != null && pipe.IsNotEmpty())
                    await sendEndpoint.Send(values, new PublishSendPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
                else
                    await sendEndpoint.Send<T>(values, cancellationToken).ConfigureAwait(false);
            }

            return PublishAsync();
        }
    }
}
