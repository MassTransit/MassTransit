namespace MassTransit.Azure.Cosmos.Table.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;


    [TestFixture]
    public class Saving_audit_records_with_custom_partitionKey :
        AzureCosmosTableInMemoryTestFixture
    {
        [Test]
        public async Task Should_Have_Custom_PartitionKey()
        {
            _records = AzureTableHelpers.GetAuditRecords().ToList();
            _records.Count.ShouldBe(1);
            _records[0].PartitionKey.ShouldBe(PartitionKey);
        }

        List<AuditRecord> _records;
        readonly string PartitionKey = "TestPartitionKey";

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await InputQueueSendEndpoint.Send(new A());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseAzureCosmosTableAuditStore(configure => configure.WithConnectionString(ConnectionString)
                                                                             .WithTableName(AuditTableName)
                                                                             .WithCustomPartitionKey((messageType, record) => PartitionKey)
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
