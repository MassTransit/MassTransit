namespace MassTransit.Transactions
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Transactions;
    using EndpointConfigurators;
    using GreenPipes;
    using Topology;


    public class TransactionalBus :
        IBus
    {
        readonly IBus _bus;
        readonly ConcurrentDictionary<Transaction, TransactionalBusEnlistment> _pendingActions;

        public TransactionalBus(IBus bus)
        {
            _bus = bus;
            _pendingActions = new ConcurrentDictionary<Transaction, TransactionalBusEnlistment>();
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _bus.ConnectPublishObserver(observer);
        }

        public async Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return new TransactionalSendEndpoint(this, await _bus.GetPublishSendEndpoint<T>());
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _bus.Publish(message, cancellationToken));
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _bus.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _bus.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            return Outbox(() => _bus.Publish(message, cancellationToken));
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return Outbox(() => _bus.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return Outbox(() => _bus.Publish(message, messageType, cancellationToken));
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return Outbox(() => _bus.Publish(message, messageType, publishPipe, cancellationToken));
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _bus.Publish<T>(values, cancellationToken));
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _bus.Publish(values, publishPipe, cancellationToken));
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Outbox(() => _bus.Publish<T>(values, publishPipe, cancellationToken));
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _bus.ConnectSendObserver(observer);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return new TransactionalSendEndpoint(this, await _bus.GetSendEndpoint(address));
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _bus.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _bus.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _bus.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _bus.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _bus.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _bus.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _bus.ConnectEndpointConfigurationObserver(observer);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return _bus.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            return _bus.ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public void Probe(ProbeContext context)
        {
            _bus.Probe(context);
        }

        public Uri Address => _bus.Address;
        public IBusTopology Topology => _bus.Topology;

        void ClearTransaction(Transaction transaction)
        {
            if (_pendingActions.TryRemove(transaction, out _))
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

        TransactionalBusEnlistment GetOrCreateEnlistment()
        {
            return _pendingActions.GetOrAdd(Transaction.Current, transaction =>
            {
                var transactionEnlistment = new TransactionalBusEnlistment();

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
