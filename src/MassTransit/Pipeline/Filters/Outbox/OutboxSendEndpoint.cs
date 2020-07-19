namespace MassTransit.Pipeline.Filters.Outbox
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Util;


    public class OutboxSendEndpoint :
        ISendEndpoint
    {
        readonly ISendEndpoint _endpoint;
        readonly OutboxContext _outboxContext;

        /// <summary>
        /// Creates an send endpoint on the outbox
        /// </summary>
        /// <param name="outboxContext">The outbox context for this consume operation</param>
        /// <param name="endpoint">The actual endpoint returned by the transport</param>
        public OutboxSendEndpoint(OutboxContext outboxContext, ISendEndpoint endpoint)
        {
            _outboxContext = outboxContext;
            _endpoint = endpoint;
        }

        /// <summary>
        /// The actual endpoint, wrapped by the outbox
        /// </summary>
        internal ISendEndpoint Endpoint => _endpoint;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        Task ISendEndpoint.Send<T>(T message, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send(object message, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, messageType, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(message, messageType, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(object values, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send<T>(values, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send(values, pipe, cancellationToken));

            return TaskUtil.Completed;
        }

        Task ISendEndpoint.Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            _outboxContext.Add(() => _endpoint.Send<T>(values, pipe, cancellationToken));

            return TaskUtil.Completed;
        }
    }
}
