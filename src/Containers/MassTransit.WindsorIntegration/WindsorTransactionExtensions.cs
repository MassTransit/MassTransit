namespace MassTransit
{
    using Castle.MicroKernel.Registration;
    using Transactions;
    using WindsorIntegration;


    public static class WindsorTransactionExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IWindsorContainerBusConfigurator builder)
        {
            builder.Container.Register(
                Component.For<TransactionalBus>()
                    .UsingFactoryMethod(kernel => new TransactionalBus(kernel.Resolve<IBus>()))
                    .LifestyleSingleton()
            );
        }
    }
}
