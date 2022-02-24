namespace MassTransit.Transactions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class TransactionalBusSendEndpoint :
        ISendEndpoint
    {
        readonly ISendEndpoint _sendEndpoint;
        readonly BaseTransactionalBus _outboxBus;

        public TransactionalBusSendEndpoint(BaseTransactionalBus outboxBus, ISendEndpoint sendEndpoint)
        {
            _outboxBus = outboxBus;
            _sendEndpoint = sendEndpoint;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpoint.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _sendEndpoint.Send(message, cancellationToken));
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _sendEndpoint.Send(message, pipe, cancellationToken));
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _sendEndpoint.Send(message, pipe, cancellationToken));
        }

        public Task Send(object message, CancellationToken cancellationToken = default)
        {
            return _outboxBus.Add(() => _sendEndpoint.Send(message, cancellationToken));
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return _outboxBus.Add(() => _sendEndpoint.Send(message, messageType, cancellationToken));
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            return _outboxBus.Add(() => _sendEndpoint.Send(message, pipe, cancellationToken));
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            return _outboxBus.Add(() => _sendEndpoint.Send(message, messageType, pipe, cancellationToken));
        }

        public Task Send<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _sendEndpoint.Send<T>(values, cancellationToken));
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _sendEndpoint.Send(values, pipe, cancellationToken));
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _sendEndpoint.Send<T>(values, pipe, cancellationToken));
        }
    }
}
