namespace MassTransit
{
    using Autofac;
    using AutofacIntegration;
    using Microsoft.Extensions.Logging;
    using Transactions;


    public static class AutofacTransactionOutboxExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IContainerBuilderConfigurator builder)
        {
            builder.Builder
                .Register(c =>
                {
                    var busControl = c.Resolve<IBusControl>();
                    return new TransactionOutbox(busControl, busControl, c.ResolveOptional<ILoggerFactory>());
                })
                .SingleInstance()
                .AsImplementedInterfaces();
        }
    }
}
