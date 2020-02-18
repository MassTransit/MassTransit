using GreenPipes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransit.Transactions
{
    public class TransactionOutboxSendEndpoint : ISendEndpoint
    {
        private readonly TransactionOutbox _transactionOutbox;
        private readonly ISendEndpoint _sendEndpoint;

        public TransactionOutboxSendEndpoint(TransactionOutbox transactionOutbox, ISendEndpoint sendEndpoint)
        {
            _transactionOutbox = transactionOutbox;
            _sendEndpoint = sendEndpoint;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
            => _sendEndpoint.ConnectSendObserver(observer);

        public Task Send<T>(T message, CancellationToken cancellationToken = default) where T : class
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send<T>(message, cancellationToken));

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default) where T : class
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send<T>(message, pipe, cancellationToken));

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default) where T : class
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send<T>(message, pipe, cancellationToken));

        public Task Send(object message, CancellationToken cancellationToken = default)
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send(message, cancellationToken));

        public Task Send(object message, Type messageType, CancellationToken cancellationToken = default)
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send(message, messageType, cancellationToken));

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send(message, pipe, cancellationToken));

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = default)
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send(message, messageType, pipe, cancellationToken));

        public Task Send<T>(object values, CancellationToken cancellationToken = default) where T : class
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send<T>(values, cancellationToken));

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = default) where T : class
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send<T>(values, pipe, cancellationToken));

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = default) where T : class
            => _transactionOutbox.Outbox(() => _sendEndpoint.Send<T>(values, pipe, cancellationToken));
    }
}
