namespace MassTransit
{
    using System;
    using Configuration;


    public static class TransactionConfiguratorExtensions
    {
        /// <summary>
        /// Encapsulate the pipe behavior in a transaction
        /// </summary>
        /// <typeparam name="T">The pipe context type</typeparam>
        /// <param name="configurator">The pipe configurator</param>
        /// <param name="configure">Configure the transaction pipe</param>
        public static void UseTransaction<T>(this IPipeConfigurator<T> configurator, Action<ITransactionConfigurator> configure = null)
            where T : class, PipeContext
        {
            var transactionConfigurator = new TransactionPipeSpecification<T>();

            configure?.Invoke(transactionConfigurator);

            configurator.AddPipeSpecification(transactionConfigurator);
        }
    }
}
