namespace MassTransit.Pipeline.Filters.Outbox
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Util;


    public class OutboxPublishEndpoint :
        IPublishEndpoint
    {
        readonly IPublishEndpoint _publishEndpoint;
        readonly OutboxContext _outboxContext;

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
            _outboxContext.Add(() => _publishEndpoint.Publish(message, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish(message, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish(message, messageType, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish(message, messageType, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish<T>(values, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish(values, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _publishEndpoint.Publish<T>(values, pipe, cancellationToken));

            return TaskUtil.Completed;
        }
    }
}
