namespace MassTransit
{
    using SimpleInjectorIntegration;
    using Transactions;


    public static class SimpleInjectorTransactionExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this ISimpleInjectorBusConfigurator builder)
        {
            builder.Container.RegisterSingleton(() => new TransactionalBus(builder.Container.GetInstance<IBus>()));
        }
    }
}
