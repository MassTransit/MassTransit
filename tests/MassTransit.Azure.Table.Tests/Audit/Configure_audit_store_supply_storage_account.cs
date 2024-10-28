using Azure.Data.Tables;

namespace MassTransit.Azure.Table.Tests.Audit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureTable;
    using NUnit.Framework;


    [TestFixture]
    public class Configure_audit_store_supply_storage_account :
        AzureTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_have_send_audit_records()
        {
            List<AuditRecord> sendRecords = GetRecords<AuditRecord>().Where(x => x.ContextType == "Send").ToList();
            Assert.That(sendRecords, Has.Count.EqualTo(1));
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var tableServiceClient = new TableServiceClient(ConnectionString);
            configurator.UseAzureTableAuditStore(tableServiceClient, TestTableName);
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
