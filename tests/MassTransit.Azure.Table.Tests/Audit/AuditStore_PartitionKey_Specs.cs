namespace MassTransit.Azure.Table.Tests.Audit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AzureTable;
    using global::Azure.Data.Tables;
    using NUnit.Framework;


    [TestFixture]
    public class Saving_audit_records_with_custom_partitionKey :
        AzureTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_Have_Custom_PartitionKey()
        {
            _records = GetTableEntities().ToList();
            Assert.That(_records, Has.Count.EqualTo(1));
            Assert.That(_records[0].PartitionKey, Is.EqualTo(PartitionKey));
        }

        List<TableEntity> _records;
        readonly string PartitionKey = "TestPartitionKey";

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseAzureTableAuditStore(TestCloudTable, new ConstantPartitionKeyFormatter(PartitionKey));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            EndpointConvention.Map<A>(new Uri($"{configurator.InputAddress}"));
        }


        class ConstantPartitionKeyFormatter :
            IPartitionKeyFormatter
        {
            readonly string _partitionKey;

            public ConstantPartitionKeyFormatter(string partitionKey)
            {
                _partitionKey = partitionKey;
            }

            public string Format<T>(AuditRecord record)
                where T : class
            {
                return _partitionKey;
            }
        }


        class A
        {
        }
    }
}
