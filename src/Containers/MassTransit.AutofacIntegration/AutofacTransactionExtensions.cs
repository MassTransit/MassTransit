namespace MassTransit.Transactions
{
    using Autofac;
    using AutofacIntegration;


    public static class AutofacTransactionExtensions
    {
        /// <summary>
        /// Adds <see cref="ITransactionalBus"/> to the container, which can be used instead of <see cref="IBus"/> to enlist
        /// published/sent messages in the current transaction. It isn't truly transactional, but delays the messages until
        /// the transaction being to commit. This has a very limited purpose and is not meant for general use.
        /// </summary>
        public static void AddTransactionalBus(this IContainerBuilderBusConfigurator builder)
        {
            builder.Builder.Register(c => new TransactionalBus(c.Resolve<IBus>()))
                .SingleInstance()
                .As<ITransactionalBus>();
        }
    }
}
