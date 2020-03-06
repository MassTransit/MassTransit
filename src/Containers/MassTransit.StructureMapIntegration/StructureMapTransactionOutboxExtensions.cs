namespace MassTransit
{
    using Microsoft.Extensions.Logging;
    using StructureMapIntegration;
    using Transactions;


    public static class StructureMapTransactionOutboxExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IConfigurationExpressionConfigurator builder)
        {
            builder.Builder
                .For<TransactionOutbox>()
                .Use("Fetch Bus Control and Create TransactionOutbox", x =>
                {
                    var busControl = x.GetInstance<IBusControl>();

                    return new TransactionOutbox(busControl, busControl, x.TryGetInstance<ILoggerFactory>());
                })
                .Singleton();

            builder.Builder
                .For<IPublishEndpoint>()
                .Use(x => x.GetInstance<TransactionOutbox>())
                .Singleton();

            builder.Builder
                .For<ISendEndpointProvider>()
                .Use(x => x.GetInstance<TransactionOutbox>())
                .Singleton();
        }
    }
}
