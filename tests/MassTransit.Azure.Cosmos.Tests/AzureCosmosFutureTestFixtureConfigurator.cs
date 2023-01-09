namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using TestFramework;


    public class AzureCosmosFutureTestFixtureConfigurator :
        IFutureTestFixtureConfigurator
    {
        const string DatabaseId = "sagaTest";
        const string CollectionId = "futurestate";

        readonly IAzureCosmosTestAuthenticationConfigurator _authConfigurator;

        public AzureCosmosFutureTestFixtureConfigurator(IAzureCosmosTestAuthenticationConfigurator authConfigurator = null)
        {
            _authConfigurator = authConfigurator ?? new AzureCosmosTestAccountKeyConfigurator();
        }

        public void ConfigureFutureSagaRepository(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaRepository<FutureState>()
                .CosmosRepository(r =>
                {
                    _authConfigurator.Configure(r);

                    r.DatabaseId = DatabaseId;
                    r.CollectionId = CollectionId;
                });
        }

        public void ConfigureServices(IServiceCollection collection)
        {
        }

        public async Task OneTimeSetup(IServiceProvider provider)
        {
            using var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<ICosmosClientFactory>().GetCosmosClient<FutureState>("client");

            var dbResponse = await client.CreateDatabaseIfNotExistsAsync(DatabaseId);
            var database = dbResponse.Database;

            await database.CreateContainerIfNotExistsAsync(CollectionId, "/id");
        }

        public Task OneTimeTearDown(IServiceProvider provider)
        {
            return Task.CompletedTask;
        }
    }
}
