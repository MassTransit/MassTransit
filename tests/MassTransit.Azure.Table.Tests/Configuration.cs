namespace MassTransit.Azure.Table.Tests
{
    using System;
    using NUnit.Framework;


    public static class Configuration
    {
        public static string StorageAccount =>
            TestContext.Parameters.Exists(nameof(StorageAccount))
                ? TestContext.Parameters.Get(nameof(StorageAccount))
                : Environment.GetEnvironmentVariable("MT_AZURE_STORAGE_ACCOUNT")
                ?? "DefaultEndpointsProtocol=http;AccountName=localhost;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;TableEndpoint=https://localhost:8081/;";
    }
}
