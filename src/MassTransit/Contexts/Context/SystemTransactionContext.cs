namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using System.Transactions;


    public class SystemTransactionContext :
        TransactionContext,
        IDisposable
    {
        readonly CommittableTransaction _transaction;
        bool _completed;
        bool _disposed;

        public SystemTransactionContext(TransactionOptions options)
        {
            _transaction = new CommittableTransaction(options);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _transaction.Dispose();

            _disposed = true;
        }

        public Transaction Transaction => _transaction;

        public async Task Commit()
        {
            if (_completed)
                return;

            await Task.Factory.FromAsync(_transaction.BeginCommit, _transaction.EndCommit, null).ConfigureAwait(false);

            _completed = true;
        }

        public void Rollback()
        {
            if (_completed)
                return;

            _transaction.Rollback();

            _completed = true;
        }

        public void Rollback(Exception exception)
        {
            if (_completed)
                return;

            _transaction.Rollback(exception);

            _completed = true;
        }
    }
}
