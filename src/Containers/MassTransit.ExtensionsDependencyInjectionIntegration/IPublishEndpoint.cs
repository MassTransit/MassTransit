namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public interface IPublishEndpoint<in TBus> :
        IPublishEndpoint
        where TBus : class
    {
    }


    class PublishEndpoint<TBus> : IPublishEndpoint<TBus>
        where TBus : class
    {
        readonly IPublishEndpoint _publishEndpoint;
        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpoint.ConnectPublishObserver(observer);
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return _publishEndpoint.Publish(message, messageType, cancellationToken);
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return _publishEndpoint.Publish<T>(values, cancellationToken);
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _publishEndpoint.Publish(values, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken);
        }

        public PublishEndpoint(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
    }
}
