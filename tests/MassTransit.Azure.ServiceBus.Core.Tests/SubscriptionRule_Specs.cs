namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Azure;
    using global::Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    public interface ClientUpdated
    {
        string Value { get; }
    }


    [TestFixture]
    public class Specifying_a_subscription_rule :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_update_the_rule_action_expression()
        {
            var topicName = "masstransit.azure.servicebus.core.tests.topologytesttypes/messageb";
            var subscriptionName = "test-sub";
            var ruleName = "test-rule";
            var queueName = "subscription-input-queue";
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

                x.ReceiveEndpoint(queueName, e =>
                {
                    e.Subscribe(topicName, subscriptionName, cb =>
                    {
                        var actionRule = new CreateRuleOptions(ruleName, new SqlRuleFilter("0 = 1")) { Action = new SqlRuleAction("SET A = 1") };

                        cb.Rule = actionRule;
                    });
                });
            });

            var busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            Response<RuleProperties> ruleResults = await managementClient.GetRuleAsync(topicName, subscriptionName, ruleName);

            Assert.That(ruleResults?.Value, Is.Not.Null);
            var rule = ruleResults.Value;
            Assert.That(rule.Action, Is.InstanceOf<SqlRuleAction>());

            var action = rule.Action as SqlRuleAction;
            Assert.That(action.SqlExpression, Is.EqualTo("SET A = 1"));

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
                    e.Subscribe(topicName, subscriptionName, cb =>
                    {
                        var actionRule = new CreateRuleOptions(ruleName, new SqlRuleFilter("1 = 1")) { Action = new SqlRuleAction("SET A = 2") };

                        cb.Rule = actionRule;
                    });
                });
            });

            busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            ruleResults = await managementClient.GetRuleAsync(topicName, subscriptionName, ruleName);

            Assert.That(ruleResults?.Value, Is.Not.Null);
            rule = ruleResults.Value;
            Assert.That(rule.Action, Is.InstanceOf<SqlRuleAction>());

            action = rule.Action as SqlRuleAction;
            Assert.That(action.SqlExpression, Is.EqualTo("SET A = 2"));
        }

        [Test]
        public async Task Should_update_the_rule_filter_expression()
        {
            var topicName = "masstransit.azure.servicebus.core.tests.topologytesttypes/messageb";
            var subscriptionName = "test-sub";
            var ruleName = "test-rule";
            var queueName = "subscription-input-queue";
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

                x.ReceiveEndpoint(queueName, e =>
                {
                    e.Subscribe(topicName, subscriptionName, cb =>
                    {
                        cb.Rule = new CreateRuleOptions(ruleName, new SqlRuleFilter("0 = 1"));
                    });
                });
            });

            var busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            Response<RuleProperties> ruleResults = await managementClient.GetRuleAsync(topicName, subscriptionName, ruleName);

            Assert.That(ruleResults?.Value, Is.Not.Null);
            var rule = ruleResults.Value;
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

                x.ReceiveEndpoint(queueName, e =>
                {
                    e.Subscribe(topicName, subscriptionName, cb =>
                    {
                        cb.Rule = new CreateRuleOptions(ruleName, new SqlRuleFilter("1 = 1"));
                    });
                });
            });

            busHandle = await bus.StartAsync();
            await busHandle.StopAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            ruleResults = await managementClient.GetRuleAsync(topicName, subscriptionName, ruleName);

            Assert.That(ruleResults?.Value, Is.Not.Null);
            rule = ruleResults.Value;
            Assert.That(rule.Filter, Is.InstanceOf<SqlRuleFilter>());

            filter = rule.Filter as SqlRuleFilter;
            Assert.That(filter.SqlExpression, Is.EqualTo("1 = 1"));
        }

        public Specifying_a_subscription_rule()
            : base(new InMemoryTestHarness())
        {
        }
    }
}
