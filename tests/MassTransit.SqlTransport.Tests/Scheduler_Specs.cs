namespace MassTransit.DbTransport.Tests;

using System;
using System.Threading.Tasks;
using Internals;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture(typeof(PostgresDatabaseTestConfiguration))]
[TestFixture(typeof(SqlServerDatabaseTestConfiguration))]
public class Canceling_a_scheduled_message_from_a_consumer<T>
    where T : IDatabaseTestConfiguration, new()
{
    [Test]
    public async Task Should_be_supported()
    {
        var testId = NewId.NextGuid();

        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSqlMessageScheduler();

                x.AddHandler<FirstMessage>(async context =>
                    {
                        ScheduledMessage<SecondMessage> scheduledMessage =
                            await context.ScheduleSend(TimeSpan.FromSeconds(5), new SecondMessage { Id = testId });

                        await Task.Delay(1000);

                        await context.CancelScheduledSend(scheduledMessage);
                    })
                    .Endpoint(e => e.Name = "schedule-input");

                x.AddHandler<SecondMessage>(async context =>
                    {
                        await Task.Delay(1, context.CancellationToken);
                    })
                    .Endpoint(e => e.Name = "schedule-input");

                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.UseSqlMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await harness.Bus.Publish(new FirstMessage());

        Assert.That(await harness.Consumed.Any<FirstMessage>());
        Assert.That(await harness.Sent.Any<SecondMessage>(x => x.Context.Message.Id == testId));

        Assert.That(async () => await harness.Consumed.Any<SecondMessage>().OrTimeout(TimeSpan.FromSeconds(5)), Throws.TypeOf<TimeoutException>());
    }

    [Test]
    public async Task Should_be_supported_from_outside_the_consumer()
    {
        var testId = NewId.NextGuid();

        Guid? tokenId = default;
        Uri destinationAddress = null;

        await using var provider = _configuration.Create()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSqlMessageScheduler();

                x.AddHandler<FirstMessage>(async context =>
                {
                    ScheduledMessage<SecondMessage> scheduledMessage = await context.ScheduleSend(TimeSpan.FromSeconds(5), new SecondMessage { Id = testId });

                    tokenId = scheduledMessage.TokenId;
                    destinationAddress = context.ReceiveContext.InputAddress;
                }).Endpoint(e => e.Name = "schedule-input");

                x.AddHandler<SecondMessage>(async context =>
                    {
                        await Task.Delay(1, context.CancellationToken);
                    })
                    .Endpoint(e => e.Name = "schedule-input");

                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                _configuration.Configure(x, (context, cfg) =>
                {
                    cfg.UseSqlMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await harness.Bus.Publish(new FirstMessage());

        Assert.That(await harness.Consumed.Any<FirstMessage>());
        Assert.That(await harness.Sent.Any<SecondMessage>(x => x.Context.Message.Id == testId));

        await Task.Delay(500);

        var scheduler = harness.Scope.ServiceProvider.GetRequiredService<IMessageScheduler>();
        await scheduler.CancelScheduledSend(destinationAddress, tokenId.Value, harness.CancellationToken);

        Assert.That(async () => await harness.Consumed.Any<SecondMessage>().OrTimeout(TimeSpan.FromSeconds(5)), Throws.TypeOf<TimeoutException>());
    }

    readonly T _configuration;

    public Canceling_a_scheduled_message_from_a_consumer()
    {
        _configuration = new T();
    }


    public record FirstMessage;


    public record SecondMessage
    {
        public Guid Id { get; init; }
    }
}
