namespace MassTransit.Azure.Cosmos.Tests
{
    public class AzureCosmosTestAccountKeyConfigurator :
        IAzureCosmosTestAuthenticationConfigurator
    {
        public void Configure(ICosmosSagaRepositoryConfigurator configurator)
        {
            configurator.AccountEndpoint = Configuration.AccountEndpoint;
            configurator.AuthKeyOrResourceToken = Configuration.AccountKey;
        }
    }
}
