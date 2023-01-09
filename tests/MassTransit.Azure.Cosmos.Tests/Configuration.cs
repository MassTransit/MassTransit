namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using NUnit.Framework;


    public static class Configuration
    {
        public static string AccountEndpoint =>
            TestContext.Parameters.Exists("CosmosAccountEndpoint")
                ? TestContext.Parameters.Get("CosmosAccountEndpoint")
                : Environment.GetEnvironmentVariable("MT_COSMOS_ENDPOINT")
                ?? AzureCosmosEmulatorConstants.AccountEndpoint;

        public static string AccountKey =>
            TestContext.Parameters.Exists("CosmosAccountKey")
                ? TestContext.Parameters.Get("CosmosAccountKey")
                : Environment.GetEnvironmentVariable("MT_COSMOS_KEY")
                ?? AzureCosmosEmulatorConstants.AccountKey;

        public static string ConnectionString => $"AccountEndpoint={AccountEndpoint};AccountKey={AccountKey}";
    }
}
