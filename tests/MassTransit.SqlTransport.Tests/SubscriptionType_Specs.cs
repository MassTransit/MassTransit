namespace MassTransit.SqlTransport.Tests;

using System;
using System.Threading.Tasks;
using DbTransport.Tests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture]
public class When_routing_via_a_routing_key
{
    [Test]
    public async Task Should_support_routing_key()
    {
        await using var provider = new ServiceCollection()
            .ConfigurePostgresTransport()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<CustomerEventConsumer>();
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                x.UsingPostgres((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("input-queue", e =>
                    {
                        e.ConfigureConsumeTopology = false;

                        e.Subscribe<CustomerUpdatedEvent>(m =>
                        {
                            m.SubscriptionType = SqlSubscriptionType.RoutingKey;
                            m.RoutingKey = "8675309";
                        });

                        e.Subscribe<CustomerDeletedEvent>(m =>
                        {
                            m.SubscriptionType = SqlSubscriptionType.RoutingKey;
                            m.RoutingKey = "655321";
                        });

                        e.ConfigureConsumer<CustomerEventConsumer>(context);
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        await harness.Bus.Publish(new CustomerUpdatedEvent(NewId.NextGuid(), "11223344"), x => x.SetRoutingKey("11223344"));
        await harness.Bus.Publish(new CustomerDeletedEvent(NewId.NextGuid(), "655321"), x => x.SetRoutingKey("655321"));

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(await harness.Consumed.Any<CustomerUpdatedEvent>(), Is.False);
            Assert.That(await harness.Consumed.Any<CustomerDeletedEvent>(), Is.True);
        });

        await harness.Stop();
    }


    class CustomerEventConsumer :
        IConsumer<CustomerUpdatedEvent>,
        IConsumer<CustomerDeletedEvent>
    {
        public Task Consume(ConsumeContext<CustomerDeletedEvent> context)
        {
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<CustomerUpdatedEvent> context)
        {
            return Task.CompletedTask;
        }
    }


    public record CustomerUpdatedEvent(Guid CorrelationId, string CustomerNumber);


    public record CustomerDeletedEvent(Guid CorrelationId, string CustomerNumber);
}


[TestFixture]
public class When_routing_using_a_pattern
{
    [Test]
    public async Task Should_support_routing_key()
    {
        await using var provider = new ServiceCollection()
            .ConfigurePostgresTransport()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ClientEventConsumer>();
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                x.UsingPostgres((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("input-queue", e =>
                    {
                        e.ConfigureConsumeTopology = false;

                        e.Subscribe<ClientUpdatedEvent>(m =>
                        {
                            m.SubscriptionType = SqlSubscriptionType.Pattern;
                            m.RoutingKey = "^[A-Z]+$";
                        });

                        e.Subscribe<ClientDeletedEvent>(m =>
                        {
                            m.SubscriptionType = SqlSubscriptionType.Pattern;
                            m.RoutingKey = "^[0-9]+$";
                        });

                        e.ConfigureConsumer<ClientEventConsumer>(context);
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        await harness.Bus.Publish(new ClientUpdatedEvent(NewId.NextGuid(), "11223344"), x => x.SetRoutingKey("11223344"));
        await harness.Bus.Publish(new ClientDeletedEvent(NewId.NextGuid(), "655321"), x => x.SetRoutingKey("655321"));

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(await harness.Consumed.Any<ClientUpdatedEvent>(), Is.False);
            Assert.That(await harness.Consumed.Any<ClientDeletedEvent>(), Is.True);
        });

        await harness.Stop();
    }


    class ClientEventConsumer :
        IConsumer<ClientUpdatedEvent>,
        IConsumer<ClientDeletedEvent>
    {
        public Task Consume(ConsumeContext<ClientDeletedEvent> context)
        {
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<ClientUpdatedEvent> context)
        {
            return Task.CompletedTask;
        }
    }


    public record ClientUpdatedEvent(Guid CorrelationId, string ClientNumber);


    public record ClientDeletedEvent(Guid CorrelationId, string ClientNumber);
}
