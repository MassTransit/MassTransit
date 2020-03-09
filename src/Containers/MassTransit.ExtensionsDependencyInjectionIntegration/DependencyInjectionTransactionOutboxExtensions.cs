namespace MassTransit
{
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Transactions;


    public static class DependencyInjectionTransactionOutboxExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IServiceCollectionConfigurator configurator)
        {
            configurator.Collection.TryAddSingleton(provider =>
            {
                var busControl = provider.GetRequiredService<IBusControl>();
                return new TransactionOutbox(busControl, busControl, provider.GetService<ILoggerFactory>());
            });

            // This should override the registrations from AddBus, in favor of the TransactionOutbox implementations
            configurator.Collection.AddSingleton<IPublishEndpoint>(x => x.GetRequiredService<TransactionOutbox>());
            configurator.Collection.AddSingleton<ISendEndpointProvider>(x => x.GetRequiredService<TransactionOutbox>());
        }
    }
}
