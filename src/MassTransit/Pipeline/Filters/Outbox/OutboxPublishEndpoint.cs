namespace MassTransit.Pipeline.Filters.Outbox
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;


    public class OutboxPublishEndpoint :
        IPublishEndpoint
    {
        readonly OutboxContext _outboxContext;
        readonly IPublishEndpoint _publishEndpoint;

        public OutboxPublishEndpoint(OutboxContext outboxContext, IPublishEndpoint publishEndpoint)
        {
            _outboxContext = outboxContext;
            _publishEndpoint = publishEndpoint;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpoint.ConnectPublishObserver(observer);
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish(message, cancellationToken));
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> pipe, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish(message, pipe, cancellationToken));
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish(message, pipe, cancellationToken));
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish(message, cancellationToken));
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish(message, messageType, cancellationToken));
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish(message, pipe, cancellationToken));
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish(message, messageType, pipe, cancellationToken));
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish<T>(values, cancellationToken));
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> pipe, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish(values, pipe, cancellationToken));
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            return _outboxContext.Add(() => _publishEndpoint.Publish<T>(values, pipe, cancellationToken));
        }
    }
}
