namespace MassTransit.DbTransport.Tests;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;
using UnitTests;


[Explicit]
[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Using_a_slow_consumer<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_renew_the_lock()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5), testTimeout: TimeSpan.FromMinutes(5));
                x.AddConsumer<SlowMessageConsumer>();

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await harness.Bus.Publish(new SlowMessage());

        Assert.That(await harness.Consumed.Any<SlowMessage>(), Is.True);
    }

    readonly T _configuration;

    public Using_a_slow_consumer()
    {
        _configuration = new T();
    }
}
