namespace MassTransit.Azure.Cosmos.Table
{
    using System;


    public static class AzureCosmosTableAuditStoreConfiguratorExtensions
    {
        public static void UseAzureCosmosTableAuditStore(this IBusFactoryConfigurator configurator,
            Func<IAzureCosmosTableConfigurator, AzureCosmosTableAuditBusObserver> configure)
        {
            var builder = new AzureCosmosTableConfigurationBuilder();
            var auditBusObserver = configure.Invoke(builder);

            configurator.ConnectBusObserver(auditBusObserver);
        }
    }
}
