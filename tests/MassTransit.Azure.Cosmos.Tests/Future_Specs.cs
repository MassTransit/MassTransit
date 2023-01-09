namespace MassTransit.Azure.Cosmos.Tests
{
    using NUnit.Framework;
    using TestFramework.ForkJoint.Tests;
    using TestFramework.Futures.Tests;


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
