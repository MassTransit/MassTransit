namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using System.Threading.Tasks;
    using global::Azure.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.ForkJoint.Tests;
    using TestFramework.Futures.Tests;


    interface IAzureCosmosTestAuthenticationConfigurator
    {
        void Configure(ICosmosSagaRepositoryConfigurator configurator);
    }

    class AzureCosmosTestAccountKeyConfigurator : IAzureCosmosTestAuthenticationConfigurator
    {
        public void Configure(ICosmosSagaRepositoryConfigurator configurator)
        {
            configurator.AccountEndpoint = Configuration.AccountEndpoint;
            configurator.AuthKeyOrResourceToken = Configuration.AccountKey;
        }
    }

    class AzureCosmosTestConnectionStringConfigurator : IAzureCosmosTestAuthenticationConfigurator
    {
        public void Configure(ICosmosSagaRepositoryConfigurator configurator)
        {
            configurator.ConnectionString = Configuration.ConnectionString;
        }
    }

    class AzureCosmosTestTokenCredentialConfigurator : IAzureCosmosTestAuthenticationConfigurator
    {
        public void Configure(ICosmosSagaRepositoryConfigurator configurator)
        {
            configurator.AccountEndpoint = Configuration.AccountEndpoint;
            configurator.TokenCredential = new DefaultAzureCredential();
        }
    }

    class AzureCosmosFutureTestFixtureConfigurator :
        IFutureTestFixtureConfigurator
    {
        const string DatabaseId = "sagaTest";
        const string CollectionId = "futurestate";

        private IAzureCosmosTestAuthenticationConfigurator _authConfigurator;

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

    // In order to get this test to pass, you have to:
    // 1. Create a CosmosDB Resource in Azure
    // 2. Configure role-based access Cosmos (https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac)
    // 3. Create a 'sagaTest' Database w/ a 'futurestate' collection (rbac doesn't support database/container management operations)
    // 4. Set Configuration.AccountEndpoint to the CosmosDB Resource (it defaults to the emulator)
    [TestFixture]
    [Ignore("Requires AzureCredentials be available on the device and that a remote Cosmos instance is prepped for RBAC.")]
    public class AzureCosmosTokenCredentialFryFutureSpecs :
        FryFuture_Specs
    {
        public AzureCosmosTokenCredentialFryFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator(new AzureCosmosTestTokenCredentialConfigurator()))
        {
        }
    }

    [TestFixture]
    public class AzureCosmosConnectionStringFryFutureSpecs :
        FryFuture_Specs
    {
        public AzureCosmosConnectionStringFryFutureSpecs()
            : base(new AzureCosmosFutureTestFixtureConfigurator(new AzureCosmosTestConnectionStringConfigurator()))
        {
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
