namespace MassTransit.Azure.Cosmos.Table.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Configure_audit_store_bring_your_own_table :
        AzureCosmosTableInMemoryTestFixture
    {
        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseAzureCosmosTableAuditStore(configure => configure.WithConnectionString(ConnectionString)
                                                                             .WithTableName(AuditTableName)
                                                                             .WithContextTypePartitionKeyStrategy()
                                                                             .WithNoMessageFilter()
                                                                             .Build());
            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            EndpointConvention.Map<A>(new Uri($"{configurator.InputAddress}"));
        }


        class A
        {
        }
    }
}
