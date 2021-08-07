namespace MassTransit
{
    using Autofac;
    using AutofacIntegration.MultiBus;
    using Transactions;


    public static class AutofacTransactionExtensions
    {
        /// <summary>
        /// Adds <see cref="ITransactionalBus" /> to the container with singleton lifetime, which can be used instead of <see cref="IBus" /> to enlist
        /// published/sent messages in the current transaction. It isn't truly transactional, but delays the messages until
        /// the transaction being to commit. This has a very limited purpose and is not meant for general use.
        /// </summary>
        public static void AddTransactionalEnlistmentBus<TBus>(this IContainerBuilderBusConfigurator<TBus> busConfigurator)
            where TBus : class, IBus
        {
            busConfigurator.Builder.Register<ITransactionalBus>(provider => new TransactionalEnlistmentBus(provider.Resolve<TBus>()))
                .SingleInstance();
        }

        /// <summary>
        /// Adds <see cref="ITransactionalBus" /> to the container with scoped lifetime, which can be used to release the messages to the bus
        /// immediately after a transaction commit. This has a very limited purpose and is not meant for general use.
        /// It is recommended this is scoped within a unit of work (e.g. Http Request)
        /// </summary>
        public static void AddTransactionalBus<TBus>(this IContainerBuilderBusConfigurator<TBus> busConfigurator)
            where TBus : class, IBus
        {
            busConfigurator.Builder.Register<ITransactionalBus>(provider => new TransactionalBus(provider.Resolve<TBus>()))
                .InstancePerLifetimeScope();
        }
    }
}
