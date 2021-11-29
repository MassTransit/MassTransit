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
    public class Saving_audit_records_with_custom_partitionKey :
        AzureTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_Have_Custom_PartitionKey()
        {
            _records = GetTableEntities().ToList();
            _records.Count.ShouldBe(1);
            _records[0].PartitionKey.ShouldBe(PartitionKey);
        }

        List<DynamicTableEntity> _records;
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
