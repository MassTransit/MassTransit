namespace MassTransit.Transactions
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.Transactions;

    public class TransactionalEnlistmentBus : BaseTransactionalBus, ITransactionalBus
    {
        readonly ConcurrentDictionary<Transaction, TransactionalEnlistmentNotification> _pendingActions;

        public TransactionalEnlistmentBus(IBus bus)
            : base(bus)
        {
            _pendingActions = new ConcurrentDictionary<Transaction, TransactionalEnlistmentNotification>();
        }

        void ClearTransaction(Transaction transaction)
        {
            if (_pendingActions.TryRemove(transaction, out _))
                transaction.TransactionCompleted -= TransactionCompleted;
        }

        public override Task Add(Func<Task> action)
        {
            if (Transaction.Current == null)
                return action();

            var enlistment = GetOrCreateEnlistment();

            enlistment.Add(action);

            return Task.CompletedTask;
        }

        TransactionalEnlistmentNotification GetOrCreateEnlistment()
        {
            return _pendingActions.GetOrAdd(Transaction.Current, transaction =>
            {
                var transactionEnlistment = new TransactionalEnlistmentNotification();

                transaction.TransactionCompleted += TransactionCompleted;
                transaction.EnlistVolatile(transactionEnlistment, EnlistmentOptions.None);

                return transactionEnlistment;
            });
        }

        void TransactionCompleted(object sender, TransactionEventArgs e)
        {
            ClearTransaction(e.Transaction);
        }

        public Task Release()
        {
            throw new NotImplementedException("Don't call release for transaction scope");
        }
    }
}
