namespace MassTransit.Azure.Table.Tests.Audit
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureTable;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Saving_send_records_with_filter :
        AzureTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_Have_Audit_Records()
        {
            _records = GetRecords<AuditRecord>();
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
            configurator.UseAzureTableAuditStore(TestCloudTable, builder => builder.Exclude(typeof(B)));
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
