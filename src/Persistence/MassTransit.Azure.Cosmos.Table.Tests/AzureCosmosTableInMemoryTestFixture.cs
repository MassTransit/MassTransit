namespace MassTransit.Azure.Cosmos.Table.Tests
{
    using NUnit.Framework;
    using TestFramework;

    public class AzureCosmosTableInMemoryTestFixture :
        InMemoryTestFixture
    {
        protected readonly string AuditTableName;
        protected readonly string ConnectionString;
        private protected AzureCosmosTableAuditStoreTestHelpers AzureTableHelpers;

        public AzureCosmosTableInMemoryTestFixture()
        {
            ConnectionString =
                $"DefaultEndpointsProtocol=http;AccountName=localhost;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;TableEndpoint=https://localhost:8081/;";
            AuditTableName                    = "mtaudit";
            AzureTableHelpers = new AzureCosmosTableAuditStoreTestHelpers(ConnectionString, AuditTableName);
        }

        [OneTimeTearDown]
        public void Take_it_down()
        {
            AzureTableHelpers.RemoveAuditTable();
        }
    }
}
