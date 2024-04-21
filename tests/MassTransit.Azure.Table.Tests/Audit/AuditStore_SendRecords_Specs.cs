namespace MassTransit.Azure.Table.Tests.Audit
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureTable;
    using NUnit.Framework;


    [TestFixture]
    public class Saving_send_audit_records_to_the_audit_store :
        AzureTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_Have_Audit_Records()
        {
            _records = GetRecords<AuditRecord>();
            Assert.That(_records, Is.Not.Empty);
        }

        [Test]
        public async Task Should_have_send_audit_record()
        {
            List<AuditRecord> sendRecords = _records.Where(x => x.ContextType == "Send").ToList();
            Assert.That(sendRecords, Has.Count.EqualTo(2));
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
            configurator.UseAzureTableAuditStore(TestCloudTable);
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
