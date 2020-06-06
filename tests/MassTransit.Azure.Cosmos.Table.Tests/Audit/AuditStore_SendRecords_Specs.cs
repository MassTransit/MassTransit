namespace MassTransit.Azure.Cosmos.Table.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Saving_send_audit_records_to_the_audit_store :
        AzureCosmosTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_Have_Audit_Records()
        {
            _records = AzureTableHelpers.GetAuditRecords();
            _records.ShouldNotBeEmpty();
        }

        [Test]
        public async Task Should_have_send_audit_record()
        {
            IEnumerable<AuditRecord> consumeRecords = _records.Where(x => x.ContextType == "Send");
            consumeRecords.Count().ShouldBe(2);
        }

        IEnumerable<AuditRecord> _records;

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
            await InputQueueSendEndpoint.Send(new B());
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


        class A
        {
            public string Data { get; set; }
        }


        class B
        {
            public string Data { get; set; }
        }
    }
}
