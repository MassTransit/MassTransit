namespace MassTransit
{
    using Castle.MicroKernel.Registration;
    using Microsoft.Extensions.Logging;
    using Transactions;
    using WindsorIntegration;


    public static class WindsorTransactionOutboxExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IWindsorContainerConfigurator builder)
        {
            builder.Container
                .Register(
                    Component.For<TransactionOutbox>()
                        .UsingFactoryMethod(x =>
                        {
                            var busControl = x.Resolve<IBusControl>();

                            return new TransactionOutbox(busControl, busControl, x.Resolve<ILoggerFactory>());
                        })
                        .LifestyleSingleton(),
                    Component.For<IPublishEndpoint>()
                        .Forward<ISendEndpointProvider>()
                        .UsingFactoryMethod(x => x.Resolve<TransactionOutbox>())
                        .LifestyleSingleton()
                );
        }
    }
}
