namespace MassTransit
{
    using System;
    using System.Transactions;


    public static class TransactionContextExtensions
    {
        /// <summary>
        /// Create a transaction scope using the transaction context (added by the TransactionFilter),
        /// to ensure that any transactions are carried between any threads.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(this PipeContext context)
        {
            var transactionContext = context.GetPayload<TransactionContext>();

            return new TransactionScope(transactionContext.Transaction);
        }

        /// <summary>
        /// Create a transaction scope using the transaction context (added by the TransactionFilter),
        /// to ensure that any transactions are carried between any threads.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scopeTimeout">The timespan after which the scope times out and aborts the transaction</param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(this PipeContext context, TimeSpan scopeTimeout)
        {
            var transactionContext = context.GetPayload<TransactionContext>();

            return new TransactionScope(transactionContext.Transaction, scopeTimeout);
        }

        /// <summary>
        /// Create a transaction scope using the transaction context (added by the TransactionFilter),
        /// to ensure that any transactions are carried between any threads.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scopeTimeout">The timespan after which the scope times out and aborts the transaction</param>
        /// <param name="asyncFlowOptions">Specifies whether transaction flow across thread continuations is enabled.</param>
        /// <returns></returns>
        public static TransactionScope CreateTransactionScope(this PipeContext context, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlowOptions)
        {
            var transactionContext = context.GetPayload<TransactionContext>();

            return new TransactionScope(transactionContext.Transaction, scopeTimeout, asyncFlowOptions);
        }
    }
}
