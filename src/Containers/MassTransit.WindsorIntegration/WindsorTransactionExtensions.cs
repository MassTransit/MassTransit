namespace MassTransit.Transactions
{
    using Castle.MicroKernel.Registration;
    using WindsorIntegration;


    public static class WindsorTransactionExtensions
    {
        /// <summary>
        /// Adds <see cref="ITransactionalBus"/> to the container, which can be used instead of <see cref="IBus"/> to enlist
        /// published/sent messages in the current transaction. It isn't truly transactional, but delays the messages until
        /// the transaction being to commit. This has a very limited purpose and is not meant for general use.
        /// </summary>
        public static void AddTransactionalBus(this IWindsorContainerBusConfigurator builder)
        {
            builder.Container.Register(
                Component.For<ITransactionalBus>()
                    .UsingFactoryMethod(kernel => new TransactionalBus(kernel.Resolve<IBus>()))
                    .LifestyleSingleton()
            );
        }
    }
}
