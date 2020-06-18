namespace MassTransit
{
    using StructureMapIntegration;
    using Transactions;


    public static class StructureMapTransactionExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IConfigurationExpressionBusConfigurator builder)
        {
            builder.Builder
                .For<TransactionalBus>()
                .Use("Fetch Bus Control and Create TransactionOutbox", context => new TransactionalBus(context.GetInstance<IBus>()))
                .Singleton();
        }
    }
}
