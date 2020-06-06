namespace MassTransit.Azure.Cosmos.Table.Tests
{
    using Audit;
    using NUnit.Framework;
    using TestFramework;


    public class AzureCosmosTableInMemoryTestFixture :
        InMemoryTestFixture
    {
        protected readonly string AccessKey;
        protected readonly string AccountName;
        protected readonly string AuditTableName;
        protected readonly string ConnectionString;
        protected readonly string TableEndpoint;
        private protected AzureCosmosTableAuditStoreTestHelpers AzureTableHelpers;

        public AzureCosmosTableInMemoryTestFixture()
        {
            AccountName = "localhost";
            AccessKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            TableEndpoint = "https://localhost:8081/";
            ConnectionString = $"DefaultEndpointsProtocol=http;AccountName={AccountName};AccountKey={AccessKey};TableEndpoint={TableEndpoint};";
            AuditTableName = "mtaudit";
            AzureTableHelpers = new AzureCosmosTableAuditStoreTestHelpers(ConnectionString, AuditTableName);
        }

        [OneTimeTearDown]
        public void Take_it_down()
        {
            AzureTableHelpers.RemoveAuditTable();
        }
    }
}
