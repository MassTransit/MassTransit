namespace MassTransit.AzureServiceBusTransport.Tests
{
    using NUnit.Framework;

    internal static class Configuration
    {
        public static string KeyName =>
            TestContext.Parameters.Exists(nameof(KeyName))
                ? TestContext.Parameters.Get(nameof(KeyName))
                : "MassTransitBuild";
        public static string ServiceNamespace =>
            TestContext.Parameters.Exists(nameof(ServiceNamespace))
                ? TestContext.Parameters.Get(nameof(ServiceNamespace))
                : "masstransit-build";
        public static string SharedAccessKey =>
            TestContext.Parameters.Exists(nameof(SharedAccessKey))
                ? TestContext.Parameters.Get(nameof(SharedAccessKey))
                : "xsvaZOKYkX8JI5N+spLCkI9iu102jLhWFJrf0LmNPMw=";
    }
}