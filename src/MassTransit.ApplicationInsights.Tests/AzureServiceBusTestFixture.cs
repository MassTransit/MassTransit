namespace MassTransit.ApplicationInsights.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.ServiceBus.Core;
    using Azure.ServiceBus.Core.Hosting;
    using Azure.ServiceBus.Core.Testing;
    using Microsoft.Azure.ServiceBus.Primitives;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    static class Configuration
    {
        public static string KeyName =>
            TestContext.Parameters.Exists(nameof(KeyName))
                ? TestContext.Parameters.Get(nameof(KeyName))
                : Environment.GetEnvironmentVariable("MT_ASB_NAMESPACE") ?? "MassTransitBuild";

        public static string ServiceNamespace =>
            TestContext.Parameters.Exists(nameof(ServiceNamespace))
                ? TestContext.Parameters.Get(nameof(ServiceNamespace))
                : Environment.GetEnvironmentVariable("MT_ASB_KEYNAME") ?? "masstransit-build";

        public static string SharedAccessKey =>
            TestContext.Parameters.Exists(nameof(SharedAccessKey))
                ? TestContext.Parameters.Get(nameof(SharedAccessKey))
                : Environment.GetEnvironmentVariable("MT_ASB_KEYVALUE") ?? "";
    }


    public class TestAzureServiceBusAccountSettings :
        ServiceBusTokenProviderSettings
    {
        static readonly string KeyName = Configuration.KeyName;
        static readonly string SharedAccessKey = Configuration.SharedAccessKey;
        readonly TokenScope _tokenScope;
        readonly TimeSpan _tokenTimeToLive;

        public TestAzureServiceBusAccountSettings()
        {
            _tokenTimeToLive = TimeSpan.FromDays(1);
            _tokenScope = TokenScope.Namespace;
        }

        string ServiceBusTokenProviderSettings.KeyName
        {
            get { return KeyName; }
        }

        string ServiceBusTokenProviderSettings.SharedAccessKey
        {
            get { return SharedAccessKey; }
        }

        TimeSpan ServiceBusTokenProviderSettings.TokenTimeToLive
        {
            get { return _tokenTimeToLive; }
        }

        TokenScope ServiceBusTokenProviderSettings.TokenScope
        {
            get { return _tokenScope; }
        }
    }


    [TestFixture]
    public abstract class AzureServiceBusTestFixture :
        BusTestFixture
    {
        protected AzureServiceBusTestHarness AzureServiceBusTestHarness { get; }

        public AzureServiceBusTestFixture(string inputQueueName = null, Uri serviceUri = null, ServiceBusTokenProviderSettings settings = null)
            : this(new AzureServiceBusTestHarness(
                serviceUri ?? AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace, "MassTransit.Azure.ServiceBus.Core.Tests"),
                settings?.KeyName ?? ((ServiceBusTokenProviderSettings)new TestAzureServiceBusAccountSettings()).KeyName,
                settings?.SharedAccessKey ?? ((ServiceBusTokenProviderSettings)new TestAzureServiceBusAccountSettings()).SharedAccessKey,
                inputQueueName))
        {
        }

        protected AzureServiceBusTestFixture(AzureServiceBusTestHarness harness)
            : base(harness)
        {
            AzureServiceBusTestHarness = harness;

            AzureServiceBusTestHarness.OnConfigureServiceBusBus += ConfigureServiceBusBus;
            AzureServiceBusTestHarness.OnConfigureServiceBusBusHost += ConfigureServiceBusBusHost;
            AzureServiceBusTestHarness.OnConfigureServiceBusReceiveEndpoint += ConfigureServiceBusReceiveEndpoint;
        }

        protected string InputQueueName => AzureServiceBusTestHarness.InputQueueName;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => AzureServiceBusTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => AzureServiceBusTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => AzureServiceBusTestHarness.Sent;

        protected Uri BusAddress => AzureServiceBusTestHarness.BusAddress;

        protected Uri InputQueueAddress => AzureServiceBusTestHarness.InputQueueAddress;

        [OneTimeSetUp]
        public async Task SetupAzureServiceBusTestFixture()
        {
            using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(20)))
            {
                await AzureServiceBusTestHarness.Start(source.Token);
            }
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return AzureServiceBusTestHarness.Stop();
        }

        protected virtual void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
        }

        protected virtual void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }

        protected IServiceBusHost Host => AzureServiceBusTestHarness.Host;
    }
}
