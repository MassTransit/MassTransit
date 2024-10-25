namespace MassTransit.Azure.Table.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Azure.Data.Tables;
    using NUnit.Framework;
    using TestFramework;


    public class AzureTableInMemoryTestFixture :
        InMemoryTestFixture
    {
        protected readonly string ConnectionString;
        protected readonly TableClient TestCloudTable;
        protected readonly string TestTableName;

        public AzureTableInMemoryTestFixture()
        {
            ConnectionString = Configuration.StorageAccount;
            TestTableName = "azuretabletests";

            var tableServiceClient = new TableServiceClient(ConnectionString);
            TestCloudTable = tableServiceClient.GetTableClient(TestTableName);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();
            base.ConfigureInMemoryBus(configurator);
        }

        public IEnumerable<T> GetRecords<T>()
            where T : class, ITableEntity, new()
        {
            return TestCloudTable.Query<T>().ToList();
        }

        public IEnumerable<TableEntity> GetTableEntities()
        {
            return TestCloudTable.Query<TableEntity>();
        }

        [OneTimeSetUp]
        public async Task Bring_it_up()
        {
            await TestCloudTable.CreateIfNotExistsAsync();

            IEnumerable<TableEntity> entities = GetTableEntities();
            IEnumerable<IGrouping<string, TableEntity>> groupedEntities = entities.GroupBy(e => e.PartitionKey);

            foreach (IGrouping<string, TableEntity> group in groupedEntities)
            {
                List<TableTransactionAction> batchOperations = group
                    .Select(entity => new TableTransactionAction(TableTransactionActionType.Delete, entity))
                    .ToList();

                // Execute the batch transaction
                await TestCloudTable.SubmitTransactionAsync(batchOperations);
            }
        }
    }
}
