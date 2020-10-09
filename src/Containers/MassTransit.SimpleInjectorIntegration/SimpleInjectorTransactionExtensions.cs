namespace MassTransit.Transactions
{
    using SimpleInjector;
    using SimpleInjectorIntegration;


    public static class SimpleInjectorTransactionExtensions
    {
        /// <summary>
        /// Adds <see cref="ITransactionalBus"/> to the container, which can be used instead of <see cref="IBus"/> to enlist
        /// published/sent messages in the current transaction. It isn't truly transactional, but delays the messages until
        /// the transaction being to commit. This has a very limited purpose and is not meant for general use.
        /// </summary>
        public static void AddTransactionalBus(this ISimpleInjectorBusConfigurator builder)
        {
            builder.Container.RegisterSingleton<ITransactionalBus>(() => new TransactionalBus(builder.Container.GetInstance<IBus>()));
        }

        /// <summary>
        /// Adds <see cref="IOutboxBus"/> to the container, which can be used to release the messages to the bus
        /// immediately after a transaction commit. This has a very limited purpose and is not meant for general use.
        /// It is recommended this is scoped within a unit of work (e.g. Http Request)
        /// </summary>
        public static void AddOutboxBus(this ISimpleInjectorBusConfigurator builder)
        {
            builder.Container.Register<IOutboxBus>(() => new OutboxBus(builder.Container.GetInstance<IBus>()), Lifestyle.Scoped);
        }
    }
}
