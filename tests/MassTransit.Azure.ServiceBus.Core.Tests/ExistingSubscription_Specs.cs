namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Azure;
    using global::Azure.Messaging.ServiceBus.Administration;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Explicit]
    public class Specifying_an_existing_subscription :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_not_update_when_no_changes_are_present()
        {
            var topicName = "not_updated_topic";
            var subscriptionName = "existing_subscription";
            var queueName = "existing_queue";
            var managementClient = Configuration.GetManagementClient();

            if (await managementClient.TopicExistsAsync(topicName))
                await managementClient.DeleteTopicAsync(topicName);

            if (await managementClient.QueueExistsAsync(queueName))
                await managementClient.DeleteQueueAsync(queueName);

            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

            var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

            var bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);
                x.Host(serviceUri, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    });
                });

                x.ReceiveEndpoint(queueName, e =>
                {
                    e.Subscribe(topicName, subscriptionName);
                });
            });

            var busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            Response<SubscriptionProperties> response = await managementClient.GetSubscriptionAsync(topicName, subscriptionName);

            Assert.Multiple(() =>
            {
                Assert.That(response?.Value, Is.Not.Null);

                Assert.That(Uri.IsWellFormedUriString(response.Value.ForwardTo, UriKind.Absolute));
                Assert.That(new Uri(response.Value.ForwardTo).AbsolutePath.TrimStart('/'), Is.EqualTo(queueName));
            });

            bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);
                x.Host(serviceUri, h =>
                {
                    h.NamedKey(s =>
                    {
                        s.NamedKeyCredential = settings.NamedKeyCredential;
                    });
                });

                x.ReceiveEndpoint(queueName, e =>
                {
                    e.Subscribe(topicName, subscriptionName);
                });
            });

            busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            await managementClient.GetSubscriptionAsync(topicName, subscriptionName);

            Assert.Multiple(() =>
            {
                Assert.That(response?.Value, Is.Not.Null);

                Assert.That(Uri.IsWellFormedUriString(response.Value.ForwardTo, UriKind.Absolute));
                Assert.That(new Uri(response.Value.ForwardTo).AbsolutePath.TrimStart('/'), Is.EqualTo(queueName));
            });
        }

        public Specifying_an_existing_subscription()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
