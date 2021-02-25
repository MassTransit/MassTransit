namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Azure.ServiceBus;
    using NUnit.Framework;
    using TestFramework;
    using TopologyTestTypes;


    [TestFixture]
    public class Specifying_a_subscription_filter :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_update_the_filter_expression()
        {
            var topicName = "masstransit.azure.servicebus.core.tests.topologytesttypes/messagea";
            var subscriptionName = "input-message-a";

            var managementClient = Configuration.GetManagementClient();

            await managementClient.DeleteTopicAsync(topicName);

            ServiceBusTokenProviderSettings settings = new TestAzureServiceBusAccountSettings();

            var serviceUri = AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace);

            var bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);
                x.Host(serviceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = settings.KeyName;
                        s.SharedAccessKey = settings.SharedAccessKey;
                        s.TokenTimeToLive = settings.TokenTimeToLive;
                        s.TokenScope = settings.TokenScope;
                    });
                });

                x.ReceiveEndpoint("subscription-input-queue", e =>
                {
                    e.Subscribe<MessageA>(subscriptionName, s => s.Filter = new SqlFilter("0 = 1"));
                });
            });

            var busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            IList<RuleDescription> rules = await managementClient.GetRulesAsync(topicName, subscriptionName);

            Assert.That(rules.Count, Is.EqualTo(1));
            Assert.That(rules[0].Filter, Is.InstanceOf<SqlFilter>());

            var filter = rules[0].Filter as SqlFilter;
            Assert.That(filter.SqlExpression, Is.EqualTo("0 = 1"));

            bus = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                BusTestFixture.ConfigureBusDiagnostics(x);
                x.Host(serviceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = settings.KeyName;
                        s.SharedAccessKey = settings.SharedAccessKey;
                        s.TokenTimeToLive = settings.TokenTimeToLive;
                        s.TokenScope = settings.TokenScope;
                    });
                });

                x.ReceiveEndpoint("subscription-input-queue", e =>
                {
                    e.Subscribe<MessageA>(subscriptionName, s => s.Filter = new SqlFilter("1 = 1"));
                });
            });

            busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            rules = await managementClient.GetRulesAsync(topicName, subscriptionName);

            Assert.That(rules.Count, Is.EqualTo(1));
            Assert.That(rules[0].Filter, Is.InstanceOf<SqlFilter>());

            filter = rules[0].Filter as SqlFilter;
            Assert.That(filter.SqlExpression, Is.EqualTo("1 = 1"));
        }

        public Specifying_a_subscription_filter()
            : base(new InMemoryTestHarness())
        {
        }
    }


    namespace TopologyTestTypes
    {
        public interface MessageA
        {
        }
    }
}
