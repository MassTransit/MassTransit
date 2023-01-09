namespace MassTransit.Azure.Cosmos.Tests
{
    public interface IAzureCosmosTestAuthenticationConfigurator
    {
        void Configure(ICosmosSagaRepositoryConfigurator configurator);
    }
}
