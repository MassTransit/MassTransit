namespace MassTransit.Azure.Cosmos.Table.Tests.Audit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Configure_audit_store_with_endpoint_and_key:
        AzureCosmosTableInMemoryTestFixture
    {
        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
        }

        [Test]
        public async Task Should_have_send_audit_records()
        {
            IEnumerable<AuditRecord> consumeRecords = AzureTableHelpers.GetAuditRecords().Where(x => x.ContextType == "Send");
            consumeRecords.Count().ShouldBe(1);
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
