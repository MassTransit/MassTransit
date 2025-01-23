namespace MassTransit.DbTransport.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[Explicit]
[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Using_message_delivery_count_limit<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_not_consume_the_message_after_the_limit()
    {
        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ExpiringMessageConsumer>();
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.UseSqlMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();

        await harness.Stop();

        await harness.Bus.Publish(new ExpiringMessage());


        await harness.Start();

        await Task.Delay(TimeSpan.FromSeconds(5));

        using var timeout = new CancellationTokenSource(2000);

        var count = await harness.Consumed.SelectAsync<ExpiringMessage>(timeout.Token).Count();
        Assert.That(count, Is.EqualTo(0));
    }

    readonly T _configuration;

    public Using_message_delivery_count_limit()
    {
        _configuration = new T();
    }


    public record ExpiringMessage;


    public class ExpiringMessageConsumer :
        IConsumer<ExpiringMessage>
    {
        public Task Consume(ConsumeContext<ExpiringMessage> context)
        {
            return Task.CompletedTask;
        }
    }
}
