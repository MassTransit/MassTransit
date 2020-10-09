namespace MassTransit.Transactions
{
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DependencyInjectionTransactionExtensions
    {
        /// <summary>
        /// Adds <see cref="ITransactionalBus"/> to the container, which can be used instead of <see cref="IBus"/> to enlist
        /// published/sent messages in the current transaction. It isn't truly transactional, but delays the messages until
        /// the transaction being to commit. This has a very limited purpose and is not meant for general use.
        /// </summary>
        public static void AddTransactionalBus(this IServiceCollectionBusConfigurator busConfigurator)
        {
            busConfigurator.Collection.TryAddSingleton<ITransactionalBus>(provider => new TransactionalBus(provider.GetService<IBus>()));
        }

        /// <summary>
        /// Adds <see cref="IOutboxBus"/> to the container, which can be used to release the messages to the bus
        /// immediately after a transaction commit. This has a very limited purpose and is not meant for general use.
        /// It is recommended this is scoped within a unit of work (e.g. Http Request)
        /// </summary>
        public static void AddOutboxBus(this IServiceCollectionBusConfigurator busConfigurator)
        {
            busConfigurator.Collection.TryAddScoped<IOutboxBus>(provider => new OutboxBus(provider.GetService<IBus>()));
        }
    }
}
