namespace MassTransit.Transactions
{
    using StructureMapIntegration;


    public static class StructureMapTransactionExtensions
    {
        /// <summary>
        /// Adds <see cref="ITransactionalBus"/> to the container, which can be used instead of <see cref="IBus"/> to enlist
        /// published/sent messages in the current transaction. It isn't truly transactional, but delays the messages until
        /// the transaction being to commit. This has a very limited purpose and is not meant for general use.
        /// </summary>
        public static void AddTransactionalBus(this IConfigurationExpressionBusConfigurator builder)
        {
            builder.Builder
                .For<ITransactionalBus>()
                .Use("Fetch Bus Control and Create TransactionOutbox", context => new TransactionalBus(context.GetInstance<IBus>()))
                .Singleton();
        }

        /// <summary>
        /// Adds <see cref="IOutboxBus"/> to the container, which can be used to release the messages to the bus
        /// immediately after a transaction commit. This has a very limited purpose and is not meant for general use.
        /// It is recommended this is scoped within a unit of work (e.g. Http Request)
        /// </summary>
        public static void AddOutboxBus(this IConfigurationExpressionBusConfigurator builder)
        {
            builder.Builder
                .For<IOutboxBus>()
                .Use("Fetch Bus Control and Create OutboxBus", context => new OutboxBus(context.GetInstance<IBus>()))
                .ContainerScoped();
        }
    }
}
