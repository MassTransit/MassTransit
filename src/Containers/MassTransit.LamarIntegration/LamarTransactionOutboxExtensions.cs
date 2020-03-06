namespace MassTransit
{
    using Lamar;
    using LamarIntegration;
    using Microsoft.Extensions.Logging;
    using Transactions;


    public static class LamarTransactionOutboxExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IServiceRegistryConfigurator builder)
        {
            builder.Builder
                .For<TransactionOutbox>()
                .Use(x =>
                {
                    var busControl = x.GetInstance<IBusControl>();

                    return new TransactionOutbox(busControl, busControl, x.TryGetInstance<ILoggerFactory>());
                })
                .Singleton();

            builder.Builder
                .Use<TransactionOutbox>()
                .Singleton()
                .For<IPublishEndpoint>()
                .For<ISendEndpointProvider>();
        }
    }
}
