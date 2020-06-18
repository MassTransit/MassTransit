namespace MassTransit
{
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Transactions;


    public static class DependencyInjectionTransactionExtensions
    {
        /// <summary>
        /// Decorates the IPublishEndpoint and ISendEndpointProvider with a Transaction Outbox. Messages will not be
        /// released/sent until the ambient transaction is committed. This is only meant to be used outside of a consumer.
        /// If you want an outbox for Consumers, it is recommended to use the InMemoryOutbox.
        /// </summary>
        public static void AddTransactionOutbox(this IServiceCollectionBusConfigurator busConfigurator)
        {
            busConfigurator.Collection.TryAddSingleton(provider => new TransactionalBus(provider.GetService<IBus>()));
        }
    }
}
