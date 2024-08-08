namespace MassTransit.AmazonSqsTransport.Tests;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Testing;


[TestFixture]
public class ReleaseLockContext_Specs
{
    [Test]
    public async Task Should_release_subsequent_lock_contexts()
    {
        var services = new ServiceCollection();

        await using var provider = services
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<TestLockContextConsumer>();

                x.AddConfigureEndpointsCallback((context, name, cfg) =>
                {
                    if (cfg is IAmazonSqsReceiveEndpointConfigurator sqs)
                    {
                        sqs.RethrowFaultedMessages();
                        sqs.ThrowOnSkippedMessages();
                        sqs.RedeliverVisibilityTimeout = 0;
                    }
                });
                x.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.LocalstackHost();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var next = Random.Shared.Next(10000);
            await harness.Bus.Publish(new TestLockContextMessage() { Id = next.ToString() });

            Assert.That(await harness.Published.Any<TestLockContextRedeliveredMessage>(x => x.Context.Message.Id == next.ToString()));
        }
        finally
        {
            await harness.Stop();
        }
    }
}


public class TestLockContextConsumer :
    IConsumer<TestLockContextMessage>
{
    readonly ILogger<TestLockContextConsumer> _logger;

    public TestLockContextConsumer(ILogger<TestLockContextConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TestLockContextMessage> context)
    {
        await Task.Delay(TimeSpan.FromSeconds(1));

        var redelivered = context.ReceiveContext.Redelivered ? "redelivered" : "";
        _logger.LogInformation($"Got Message {context.Message.Id} {redelivered}");

        if (context.ReceiveContext.Redelivered)
        {
            await context.Publish(new TestLockContextRedeliveredMessage() { Id = context.Message.Id });
            return;
        }

        throw new Exception("This is intentional");
    }
}


public class TestLockContextMessage
{
    public string Id { get; set; }
}


public class TestLockContextRedeliveredMessage
{
    public string Id { get; set; }
}
