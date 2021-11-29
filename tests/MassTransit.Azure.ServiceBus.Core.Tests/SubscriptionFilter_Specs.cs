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
    using TopologyTestTypes;


    [TestFixture]
    public class Using_a_subscription_filter_on_int :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_only_match_integers()
        {
            await Bus.Publish<ClientUpdated>(new { Value = "Invalid" }, x => x.Headers.Set("ClientId", 69));
            await Bus.Publish<ClientUpdated>(new { Value = "Valid" }, x => x.Headers.Set("ClientId", 27));

            ConsumeContext<ClientUpdated> handled = await _handled;

            Assert.That(handled.Message.Value, Is.EqualTo("Valid"));
        }

        Task<ConsumeContext<ClientUpdated>> _handled;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.SubscriptionEndpoint<ClientUpdated>("client-update-27", x =>
            {
                x.PrefetchCount = 1;

                x.Rule = new CreateRuleOptions("Only27", new SqlRuleFilter("ClientId = 27"));

                _handled = Handled<ClientUpdated>(x);
            });
        }
    }


    namespace TopologyTestTypes
    {
        public interface ClientUpdated
        {
            string Value { get; }
        }
    }


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

            if (await managementClient.TopicExistsAsync(topicName))
                await managementClient.DeleteTopicAsync(topicName);

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

                x.ReceiveEndpoint("subscription-input-queue", e =>
                {
                    e.Subscribe<MessageA>(subscriptionName, s => s.Filter = new SqlRuleFilter("0 = 1"));
                });
            });

            var busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            AsyncPageable<RuleProperties> pageableRules = managementClient.GetRulesAsync(topicName, subscriptionName);

            Assert.That(await pageableRules.Count(), Is.EqualTo(1));
            var rule = await pageableRules.First();
            Assert.That(rule.Filter, Is.InstanceOf<SqlRuleFilter>());

            var filter = rule.Filter as SqlRuleFilter;
            Assert.That(filter.SqlExpression, Is.EqualTo("0 = 1"));

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

                x.ReceiveEndpoint("subscription-input-queue", e =>
                {
                    e.Subscribe<MessageA>(subscriptionName, s => s.Filter = new SqlRuleFilter("1 = 1"));
                });
            });

            busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            pageableRules = managementClient.GetRulesAsync(topicName, subscriptionName);

            Assert.That(await pageableRules.Count(), Is.EqualTo(1));
            rule = await pageableRules.First();
            Assert.That(rule.Filter, Is.InstanceOf<SqlRuleFilter>());

            filter = rule.Filter as SqlRuleFilter;
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
