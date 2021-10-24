namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [Explicit]
    [TestFixture]
    public class When_a_receive_endpoint_queue_is_deleted :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_recreate_the_queue()
        {
            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();
            var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);
            var busControl = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);
                x.Host(serviceUri, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    });
                });

                x.ReceiveEndpoint("input-queue", e =>
                {
                });
            });

            var handle = await busControl.StartAsync();
            try
            {
                Console.WriteLine("Waiting for connection...");

                await handle.Ready;

                await Task.Delay(30000);
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        public When_a_receive_endpoint_queue_is_deleted()
            : base(new InMemoryTestHarness())
        {
        }
    }


    [Explicit]
    [TestFixture]
    public class When_a_subscription_endpoint_topic_or_subscription_is_deleted :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_recreate_the_subscription()
        {
            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();
            var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);
            var busControl = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);
                x.Host(serviceUri, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    });
                });

                x.SubscriptionEndpoint("input-subscription", "input-topic", e =>
                {
                });
            });

            var handle = await busControl.StartAsync();
            try
            {
                Console.WriteLine("Waiting for connection...");

                await handle.Ready;

                await Task.Delay(40000);
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        public When_a_subscription_endpoint_topic_or_subscription_is_deleted()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
