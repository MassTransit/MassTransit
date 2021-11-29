namespace MassTransit.Azure.Table.Tests.Audit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureTable;
    using Microsoft.Azure.Cosmos.Table;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Configure_audit_store_supply_storage_account :
        AzureTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_have_send_audit_records()
        {
            IEnumerable<AuditRecord> sendRecords = GetRecords<AuditRecord>().Where(x => x.ContextType == "Send");
            sendRecords.Count().ShouldBe(1);
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            configurator.UseAzureTableAuditStore(storageAccount, TestTableName);
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
