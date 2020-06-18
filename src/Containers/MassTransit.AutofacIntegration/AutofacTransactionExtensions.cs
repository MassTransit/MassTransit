namespace MassTransit
{
    using Autofac;
    using AutofacIntegration;
    using Transactions;


    public static class AutofacTransactionExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IContainerBuilderBusConfigurator builder)
        {
            builder.Builder.Register(c => new TransactionalBus(c.Resolve<IBus>()))
                .SingleInstance()
                .As<TransactionalBus>();
        }
    }
}
