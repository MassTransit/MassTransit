namespace MassTransit.Azure.Cosmos.Table.Tests.Audit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Configure_audit_store_with_endpoint_and_key :
        AzureCosmosTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_have_send_audit_records()
        {
            IEnumerable<AuditRecord> sendRecords = AzureTableHelpers.GetAuditRecords().Where(x => x.ContextType == "Send");
            sendRecords.Count().ShouldBe(1);
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseAzureCosmosTableAuditStore(configure => configure.WithAccessKey(AccountName, AccessKey, TableEndpoint)
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
