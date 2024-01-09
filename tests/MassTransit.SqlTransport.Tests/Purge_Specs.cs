namespace MassTransit.DbTransport.Tests;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;
using UnitTests;


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
[Explicit]
public class Purging_a_queue_on_startup<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_purge_the_queue()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<TestMessageConsumer>();

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ReceiveEndpoint("empty-queue", e =>
                    {
                        e.PurgeOnStartup = true;

                        e.ConfigureConsumer<TestMessageConsumer>(context);
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await harness.Bus.Publish(new TestMessage("Hello, World!"));

        Assert.That(await harness.Consumed.Any<TestMessage>());
        
        await harness.Stop();
    }

    readonly T _configuration;

    public Purging_a_queue_on_startup()
    {
        _configuration = new T();
    }
}