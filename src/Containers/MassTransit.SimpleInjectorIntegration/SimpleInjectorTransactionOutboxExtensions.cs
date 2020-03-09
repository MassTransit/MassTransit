namespace MassTransit
{
    using Microsoft.Extensions.Logging;
    using SimpleInjector;
    using SimpleInjectorIntegration;
    using Transactions;


    public static class SimpleInjectorTransactionOutboxExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this ISimpleInjectorConfigurator builder)
        {
            builder.Container
                .RegisterSingleton(() =>
                {
                    var busControl = builder.Container.GetInstance<IBusControl>();

                    return new TransactionOutbox(busControl, busControl, builder.Container.TryGetInstance<ILoggerFactory>());
                });

            builder.Container
                .Register(() => (IPublishEndpoint)builder.Container.GetInstance<TransactionOutbox>(), Lifestyle.Singleton);

            builder.Container
                .Register(() => (ISendEndpointProvider)builder.Container.GetInstance<TransactionOutbox>(), Lifestyle.Singleton);
        }
    }
}
