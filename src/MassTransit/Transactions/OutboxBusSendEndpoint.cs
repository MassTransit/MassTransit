namespace MassTransit.Transactions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    public class OutboxBusSendEndpoint :
        ISendEndpoint
    {
        readonly ISendEndpoint _sendEndpoint;
        readonly BaseOutboxBus _outboxBus;

        public OutboxBusSendEndpoint(BaseOutboxBus outboxBus, ISendEndpoint sendEndpoint)
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
            _outboxBus.Add(() => _sendEndpoint.Send(message, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            _outboxBus.Add(() => _sendEndpoint.Send(message, pipe, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            _outboxBus.Add(() => _sendEndpoint.Send(message, pipe, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send(object message, CancellationToken cancellationToken = default)
        {
            _outboxBus.Add(() => _sendEndpoint.Send(message, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            _outboxBus.Add(() => _sendEndpoint.Send(message, messageType, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            _outboxBus.Add(() => _sendEndpoint.Send(message, pipe, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
        {
            _outboxBus.Add(() => _sendEndpoint.Send(message, messageType, pipe, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            _outboxBus.Add(() => _sendEndpoint.Send<T>(values, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            _outboxBus.Add(() => _sendEndpoint.Send(values, pipe, cancellationToken));
            return Task.CompletedTask;
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            _outboxBus.Add(() => _sendEndpoint.Send<T>(values, pipe, cancellationToken));
            return Task.CompletedTask;
        }
    }
}
