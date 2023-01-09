namespace MassTransit.Azure.Cosmos.Tests
{
    public class AzureCosmosTestConnectionStringConfigurator :
        IAzureCosmosTestAuthenticationConfigurator
    {
        public void Configure(ICosmosSagaRepositoryConfigurator configurator)
        {
            configurator.ConnectionString = Configuration.ConnectionString;
        }
    }
}
