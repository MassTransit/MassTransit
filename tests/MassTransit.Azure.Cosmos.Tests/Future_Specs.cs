namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.ForkJoint.Tests;
    using TestFramework.Futures.Tests;


    class AzureCosmosFutureTestFixtureConfigurator :
        IFutureTestFixtureConfigurator
    {
        const string DatabaseId = "sagaTest";
        const string CollectionId = "futurestate";

        public void ConfigureFutureSagaRepository(IBusRegistrationConfigurator configurator)
        {
            configurator.AddSagaRepository<FutureState>()
                .CosmosRepository(r =>
                {
                    r.EndpointUri = Configuration.EndpointUri;
                    r.Key = Configuration.Key;

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


    [TestFixture]
    public class AzureCosmosFryFutureSpecs :
        FryFuture_Specs
    {
        public AzureCosmosFryFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosShakeFutureSpecs :
        ShakeFuture_Specs
    {
        public AzureCosmosShakeFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosFryShakeFutureSpecs :
        FryShakeFuture_Specs
    {
        public AzureCosmosFryShakeFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosBurgerFutureSpecs :
        BurgerFuture_Specs
    {
        public AzureCosmosBurgerFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosCalculateFutureSpecs :
        CalculateFuture_Specs
    {
        public AzureCosmosCalculateFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosOrderFutureSpecs :
        OrderFuture_Specs
    {
        public AzureCosmosOrderFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosComboFutureSpecs :
        ComboFuture_Specs
    {
        public AzureCosmosComboFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosPriceCalculationFuture_Specs :
        PriceCalculationFuture_Specs
    {
        public AzureCosmosPriceCalculationFuture_Specs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosPriceCalculationFuture_RegistrationSpecs :
        PriceCalculationFuture_RegistrationSpecs
    {
        public AzureCosmosPriceCalculationFuture_RegistrationSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }


    [TestFixture]
    public class AzureCosmosPriceCalculationFuture_Faulted :
        PriceCalculationFuture_Faulted
    {
        public AzureCosmosPriceCalculationFuture_Faulted()
            : base(new AzureCosmosFutureTestFixtureConfigurator())
        {
        }
    }
}
