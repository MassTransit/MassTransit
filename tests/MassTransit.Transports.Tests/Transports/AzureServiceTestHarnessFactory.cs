namespace MassTransit.Transports.Tests.Transports
{
    using System;
    using System.Threading.Tasks;
    using global::Azure;
    using NUnit.Framework;
    using Testing;


    public class AzureServiceBusTestHarnessFactory :
        ITestHarnessFactory
    {
        public async Task<BusTestHarness> CreateTestHarness()
        {
            return new AzureServiceBusTestHarness(Configuration.ServiceEndpoint, new AzureNamedKeyCredential(Configuration.KeyName, Configuration.SharedAccessKey));
        }


        static class Configuration
        {
            public static readonly Uri ServiceEndpoint = new Uri($"sb://{ServiceNamespace}.servicebus.windows.net/MassTransit.Transports.Tests");

            public static string KeyName =>
                TestContext.Parameters.Exists(nameof(KeyName))
                    ? TestContext.Parameters.Get(nameof(KeyName))
                    : Environment.GetEnvironmentVariable("MT_ASB_KEYNAME") ?? "MassTransitBuild";

            public static string ServiceNamespace =>
                TestContext.Parameters.Exists(nameof(ServiceNamespace))
                    ? TestContext.Parameters.Get(nameof(ServiceNamespace))
                    : Environment.GetEnvironmentVariable("MT_ASB_NAMESPACE") ?? "masstransit-build";

            public static string SharedAccessKey =>
                TestContext.Parameters.Exists(nameof(SharedAccessKey))
                    ? TestContext.Parameters.Get(nameof(SharedAccessKey))
                    : Environment.GetEnvironmentVariable("MT_ASB_KEYVALUE") ?? "YfN2b8jT84759bZy5sMhd0P+3K/qHqO81I5VrNrJYkI=";
        }
    }
}
