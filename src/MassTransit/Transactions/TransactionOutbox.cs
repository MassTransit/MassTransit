namespace MassTransit.Transactions
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using GreenPipes;
    using Microsoft.Extensions.Logging;


    public class TransactionOutbox :
        IPublishEndpoint,
        ISendEndpointProvider
    {
        readonly ILoggerFactory _loggerFactory;
        readonly ConcurrentDictionary<Transaction, TransactionOutboxEnlistment> _pendingActions;
        readonly IPublishEndpoint _publishEndpoint;
        readonly ISendEndpointProvider _sendEndpointProvider;

        public TransactionOutbox(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
            _pendingActions = new ConcurrentDictionary<Transaction, TransactionOutboxEnlistment>();
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishEndpoint.ConnectPublishObserver(observer);
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _publishEndpoint.Publish(message, cancellationToken));
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _publishEndpoint.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _publishEndpoint.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            return Outbox(() => _publishEndpoint.Publish(message, cancellationToken));
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return Outbox(() => _publishEndpoint.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return Outbox(() => _publishEndpoint.Publish(message, messageType, cancellationToken));
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return Outbox(() => _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken));
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _publishEndpoint.Publish<T>(values, cancellationToken));
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _publishEndpoint.Publish(values, publishPipe, cancellationToken));
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken));
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendEndpointProvider.ConnectSendObserver(observer);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return new TransactionOutboxSendEndpoint(this, await _sendEndpointProvider.GetSendEndpoint(address));
        }

        public void ClearTransaction(Transaction transaction)
        {
            if (_pendingActions.TryRemove(transaction, out var transactionOutboxEnlistment))
                transaction.TransactionCompleted -= TransactionCompleted;
        }

        public Task Outbox(Func<Task> action)
        {
            if (Transaction.Current == null)
                return action();

            var pendingActions = GetOrCreateEnlistment();

            pendingActions.Add(action);

            return Task.CompletedTask;
        }

        TransactionOutboxEnlistment GetOrCreateEnlistment()
        {
            return _pendingActions.GetOrAdd(Transaction.Current, transaction =>
            {
                var transactionEnlistment = new TransactionOutboxEnlistment(_loggerFactory);

                transaction.TransactionCompleted += TransactionCompleted;
                transaction.EnlistVolatile(transactionEnlistment, EnlistmentOptions.None);

                return transactionEnlistment;
            });
        }

        void TransactionCompleted(object sender, TransactionEventArgs e)
        {
            ClearTransaction(e.Transaction);
        }
    }
}
