namespace MassTransit.DbTransport.Tests;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;
using UnitTests;


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Using_publish<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_consume_a_lot_of_published_messages()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(TextWriter.Null, x =>
            {
                x.AddConsumer<TestMessageConsumer>();
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ReceiveEndpoint("publish-input-queue", e =>
                    {
                        e.PrefetchCount = 30;

                        e.ConfigureConsumer<TestMessageConsumer>(context);
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };

        var timer = Stopwatch.StartNew();

        const int limit = 1000;

        await Parallel.ForEachAsync(Enumerable.Range(0, limit), options, async (i, token) =>
        {
            await harness.Bus.Publish(new TestMessage($"Hello, World! {i}"), token);
        });

        var sendElapsed = timer.Elapsed;

        await harness.Consumed.SelectAsync<TestMessage>().Take(limit).Count();

        var consumeElapsed = timer.Elapsed;

        timer.Stop();

        Console.WriteLine("Total publish duration: {0:g}", sendElapsed);
        Console.WriteLine("Publish message rate: {0:F2} (msg/s)",
            limit * 1000 / sendElapsed.TotalMilliseconds);
        Console.WriteLine("Total consume duration: {0:g}", consumeElapsed);
        Console.WriteLine("Consume message rate: {0:F2} (msg/s)",
            limit * 1000 / consumeElapsed.TotalMilliseconds);
    }

    readonly T _configuration;

    public Using_publish()
    {
        _configuration = new T();
    }
}


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Publishing_a_unsubscribed_message_type<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_not_leave_orphaned_messages()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.ReceiveEndpoint("publish-input-queue", e =>
                    {
                        e.PrefetchCount = 30;
                    });
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        await harness.Bus.Publish(new TestMessage($"Hello, World!"), harness.CancellationToken);

        await harness.Stop();
    }

    readonly T _configuration;

    public Publishing_a_unsubscribed_message_type()
    {
        _configuration = new T();
    }
}
