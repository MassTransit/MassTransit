namespace MassTransit.Azure.Cosmos.Tests
{
    using global::Azure.Identity;


    public class AzureCosmosTestTokenCredentialConfigurator :
        IAzureCosmosTestAuthenticationConfigurator
    {
        public void Configure(ICosmosSagaRepositoryConfigurator configurator)
        {
            configurator.AccountEndpoint = Configuration.AccountEndpoint;
            configurator.TokenCredential = new DefaultAzureCredential();
        }
    }
}
