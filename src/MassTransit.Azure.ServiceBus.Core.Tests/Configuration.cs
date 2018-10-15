namespace MassTransit.Azure.ServiceBus.Core.Tests
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
                : "u07PCkrzic95bk6UtpDXl8YdHmV40WtJihoGz2CnMMQ=";
    }
}