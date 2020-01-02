using GreenPipes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace MassTransit.Transactions
{
    public class TransactionOutbox : IPublishEndpoint, ISendEndpointProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        readonly ConcurrentDictionary<Transaction, TransactionOutboxEnlistment> _pendingActions;

        public TransactionOutbox(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
            _pendingActions = new ConcurrentDictionary<Transaction, TransactionOutboxEnlistment>();
        }

        public void ClearTransaction(Transaction transaction)
        {
            if (_pendingActions.TryRemove(transaction, out var transactionOutboxEnlistment))
            {
                transaction.TransactionCompleted -= TransactionCompleted;
            }
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
            => _publishEndpoint.ConnectPublishObserver(observer);

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
            => _sendEndpointProvider.ConnectSendObserver(observer);

        public Task Outbox(Func<Task> action)
        {
            if (Transaction.Current == null)
                return action();

            var pendingActions = GetOrCreateEnlistment();

            pendingActions.Add(action);

            return Task.CompletedTask;
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
            => new TransactionOutboxSendEndpoint(this, await _sendEndpointProvider.GetSendEndpoint(address));

        public Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class
            => Outbox(() => _publishEndpoint.Publish<T>(message, cancellationToken));

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default) where T : class
            => Outbox(() => _publishEndpoint.Publish<T>(message, publishPipe, cancellationToken));

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T : class
            => Outbox(() => _publishEndpoint.Publish<T>(message, publishPipe, cancellationToken));

        public Task Publish(object message, CancellationToken cancellationToken = default)
            => Outbox(() => _publishEndpoint.Publish(message, cancellationToken));

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            => Outbox(() => _publishEndpoint.Publish(message, publishPipe, cancellationToken));

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
            => Outbox(() => _publishEndpoint.Publish(message, messageType, cancellationToken));

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            => Outbox(() => _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken));

        public Task Publish<T>(object values, CancellationToken cancellationToken = default) where T : class
            => Outbox(() => _publishEndpoint.Publish<T>(values, cancellationToken));

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default) where T : class
            => Outbox(() => _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken));

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T : class
            => Outbox(() => _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken));

        private TransactionOutboxEnlistment GetOrCreateEnlistment()
        {
            return _pendingActions.GetOrAdd(Transaction.Current, transaction =>
            {
                var transactionEnlistment = new TransactionOutboxEnlistment(_loggerFactory);

                transaction.TransactionCompleted += TransactionCompleted;
                transaction.EnlistVolatile(transactionEnlistment, EnlistmentOptions.None);

                return transactionEnlistment;
            });
        }

        private void TransactionCompleted(object sender, TransactionEventArgs e)
        {
            ClearTransaction(e.Transaction);
        }
    }
}
