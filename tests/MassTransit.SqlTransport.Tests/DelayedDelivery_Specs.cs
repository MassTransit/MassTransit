namespace MassTransit.DbTransport.Tests;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;
using UnitTests;


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Using_delayed_send<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_be_supported()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<TestMessageConsumer>();
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ReceiveEndpoint("delayed-input-queue", e =>
                    {
                        e.ConfigureConsumeTopology = false;

                        e.ConfigureConsumer<TestMessageConsumer>(context);
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var endpoint = await harness.Bus.GetSendEndpoint(new Uri("queue:delayed-input-queue"));

        var testMessage = new TestMessage("Hello, World!");

        var now = DateTime.UtcNow;

        await endpoint.Send(testMessage, x => x.Delay = TimeSpan.FromSeconds(3));

        Assert.That(await harness.Consumed.Any<TestMessage>(x => x.Context.Message.Id == testMessage.Id), Is.True);

        var then = DateTime.UtcNow;

        Assert.That(then - now, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2)));
    }

    readonly T _configuration;

    public Using_delayed_send()
    {
        _configuration = new T();
    }
}


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Using_delayed_publish<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_be_supported()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<TestMessageConsumer>();
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ReceiveEndpoint("delayed-publish-input-queue", e =>
                    {
                        e.ConfigureConsumer<TestMessageConsumer>(context);
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var testMessage = new TestMessage("Hello, World!");

        var now = DateTime.UtcNow;

        await harness.Bus.Publish(testMessage, x => x.Delay = TimeSpan.FromSeconds(3));

        Assert.That(await harness.Consumed.Any<TestMessage>(x => x.Context.Message.Id == testMessage.Id), Is.True);

        var then = DateTime.UtcNow;

        Assert.That(then - now, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(2)));
    }

    readonly T _configuration;

    public Using_delayed_publish()
    {
        _configuration = new T();
    }
}
