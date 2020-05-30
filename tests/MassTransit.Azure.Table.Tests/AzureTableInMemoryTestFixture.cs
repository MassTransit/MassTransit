namespace MassTransit.Azure.Table.Tests
{
    using System.Collections.Generic;
    using System.Linq;
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

        public IEnumerable<T> GetRecords<T>()
        {
            var query = new TableQuery();
            IEnumerable<DynamicTableEntity> entities = TestCloudTable.ExecuteQuery(query);
            return entities.Select(e => TableEntity.ConvertBack<T>(e.Properties, new OperationContext()));
        }

        public IEnumerable<DynamicTableEntity> GetTableEntities()
        {
            var query = new TableQuery();
            return TestCloudTable.ExecuteQuery(query);
        }

        [OneTimeSetUp]
        public void Bring_it_up()
        {
            TestCloudTable.CreateIfNotExists();
        }

        [OneTimeTearDown]
        public void Take_it_down()
        {
            TestCloudTable.DeleteIfExists();
        }
    }
}
