namespace MassTransit.Azure.Table.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;
    using NUnit.Framework;
    using TestFramework;


    public class AzureTableInMemoryTestFixture :
        InMemoryTestFixture
    {
        protected readonly string ConnectionString;
        protected readonly CloudTable TestCloudTable;
        protected readonly string TestTableName;

        public AzureTableInMemoryTestFixture()
        {
            ConnectionString = Configuration.StorageAccount;
            TestTableName = "azuretabletests";
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            TestCloudTable = tableClient.GetTableReference(TestTableName);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();

            base.ConfigureInMemoryBus(configurator);
        }

        public IEnumerable<T> GetRecords<T>()
        {
            IEnumerable<DynamicTableEntity> entities = TestCloudTable.ExecuteQuery(new TableQuery());
            return entities.Select(e => TableEntity.ConvertBack<T>(e.Properties, new OperationContext()));
        }

        public IEnumerable<DynamicTableEntity> GetTableEntities()
        {
            return TestCloudTable.ExecuteQuery(new TableQuery());
        }

        [OneTimeSetUp]
        public async Task Bring_it_up()
        {
            TestCloudTable.CreateIfNotExists();

            IEnumerable<DynamicTableEntity> entities = GetTableEntities();

            foreach (IGrouping<string, DynamicTableEntity> key in entities.GroupBy(x => x.PartitionKey))
            {
                // Create the batch operation.
                var batchDeleteOperation = new TableBatchOperation();

                foreach (var row in key)
                    batchDeleteOperation.Delete(row);

                // Execute the batch operation.
                await TestCloudTable.ExecuteBatchAsync(batchDeleteOperation);
            }
        }
    }
}
