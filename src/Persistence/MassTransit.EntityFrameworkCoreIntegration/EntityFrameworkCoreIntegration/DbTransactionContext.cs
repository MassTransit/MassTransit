namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;


    /// <summary>
    /// Implemented when a filter/context has already started and is managing the transaction
    /// </summary>
    public interface DbTransactionContext
    {
        Guid TransactionId { get; }
    }
}
