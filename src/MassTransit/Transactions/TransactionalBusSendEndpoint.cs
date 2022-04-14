namespace MassTransit.Transactions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;


    public class TransactionalBusSendEndpoint :
        ITransportSendEndpoint
    {
        readonly ITransportSendEndpoint _endpoint;
        readonly BaseTransactionalBus _outboxBus;

        public TransactionalBusSendEndpoint(BaseTransactionalBus outboxBus, ISendEndpoint endpoint)
        {
            _outboxBus = outboxBus;
            _endpoint = endpoint as ITransportSendEndpoint ?? throw new ArgumentException("Must be a transport endpoint", nameof(endpoint));
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        public Task<SendContext<T>> CreateSendContext<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.CreateSendContext(message, pipe, cancellationToken);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _endpoint.Send(message, cancellationToken));
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _endpoint.Send(message, pipe, cancellationToken));
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _endpoint.Send(message, pipe, cancellationToken));
        }

        public Task Send(object message, CancellationToken cancellationToken = default)
        {
            return _outboxBus.Add(() => _endpoint.Send(message, cancellationToken));
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return _outboxBus.Add(() => _endpoint.Send(message, messageType, cancellationToken));
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            return _outboxBus.Add(() => _endpoint.Send(message, pipe, cancellationToken));
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            return _outboxBus.Add(() => _endpoint.Send(message, messageType, pipe, cancellationToken));
        }

        public Task Send<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _endpoint.Send<T>(values, cancellationToken));
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _endpoint.Send(values, pipe, cancellationToken));
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _outboxBus.Add(() => _endpoint.Send<T>(values, pipe, cancellationToken));
        }
    }
}
