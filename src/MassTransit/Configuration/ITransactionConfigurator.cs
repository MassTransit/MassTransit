namespace MassTransit
{
    using System;
    using System.Transactions;


    public interface ITransactionConfigurator
    {
        /// <summary>
        /// Sets the transaction timeout
        /// </summary>
        TimeSpan Timeout { set; }

        /// <summary>
        /// Sets the isolation level of the transaction
        /// </summary>
        IsolationLevel IsolationLevel { set; }
    }
}
