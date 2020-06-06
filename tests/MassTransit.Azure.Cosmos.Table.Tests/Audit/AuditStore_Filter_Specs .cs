namespace MassTransit.Azure.Cosmos.Table.Tests.Audit
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Saving_send_records_with_filter :
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
            List<AuditRecord> sendRecords = _records.Where(x => x.ContextType == "Send").ToList();
            sendRecords.Count.ShouldBe(1);
            sendRecords[0].MessageType.ShouldBe(typeof(A).FullName);
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
                .WithMessageFilter(builder => builder.Exclude(typeof(B)))
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
