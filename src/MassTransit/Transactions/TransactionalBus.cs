namespace MassTransit.Transactions
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using System.Transactions;

    public class TransactionalBus : BaseOutboxBus, ITransactionalBus
    {
        readonly ConcurrentDictionary<Transaction, TransactionalBusEnlistment> _pendingActions;

        public TransactionalBus(IBus bus)
            : base(bus)
        {
            _pendingActions = new ConcurrentDictionary<Transaction, TransactionalBusEnlistment>();
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
