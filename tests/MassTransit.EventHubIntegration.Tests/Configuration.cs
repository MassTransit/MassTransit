namespace MassTransit.EventHubIntegration.Tests
{
    using System;
    using NUnit.Framework;


    static class Configuration
    {
        public static string EventHubNamespace =>
            TestContext.Parameters.Exists(nameof(EventHubNamespace))
                ? TestContext.Parameters.Get(nameof(EventHubNamespace))
                : Environment.GetEnvironmentVariable("MT_EH_NAMESPACE") ?? "MassTransitBuild";

        public static string EventHubName =>
            TestContext.Parameters.Exists(nameof(EventHubName))
                ? TestContext.Parameters.Get(nameof(EventHubName))
                : Environment.GetEnvironmentVariable("MT_EH_NAME") ?? "masstransit-build";

        public static string StorageAccount =>
            TestContext.Parameters.Exists(nameof(StorageAccount))
                ? TestContext.Parameters.Get(nameof(StorageAccount))
                : Environment.GetEnvironmentVariable("MT_AZURE_STORAGE_ACCOUNT") ?? "";
    }
}
