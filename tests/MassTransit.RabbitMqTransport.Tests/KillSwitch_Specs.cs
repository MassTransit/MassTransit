namespace MassTransit.RabbitMqTransport.Tests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework;
using Testing;


[Category("Flaky")]
[TestFixture]
public class Using_the_kill_switch_with_rabbitmq
{
    [Test]
    public async Task Should_be_degraded_after_too_many_exceptions()
    {
        await using var provider = new ServiceCollection()
            .ConfigureRabbitMqTestOptions(options =>
            {
                options.CleanVirtualHost = true;
                options.CreateVirtualHostIfNotExists = true;
            })
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<MessageConsumer>();

                x.AddOptions<RabbitMqTransportOptions>()
                    .Configure(options => options.VHost = "test");

                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.PrefetchCount = 20;
                    cfg.ConcurrentMessageLimit = 1;

                    cfg.UseKillSwitch(options => options
                        .SetActivationThreshold(9)
                        .SetTripThreshold(10)
                        .SetRestartTimeout(s: 5));

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var busControl = provider.GetRequiredService<IBusControl>();

        Assert.That(await busControl.WaitForHealthStatus(BusHealthStatus.Healthy, TimeSpan.FromSeconds(5)), Is.EqualTo(BusHealthStatus.Healthy));

        await harness.Bus.PublishBatch(Enumerable.Range(0, 20).Select(_ => new MessageA()));

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(await busControl.WaitForHealthStatus(BusHealthStatus.Degraded, TimeSpan.FromSeconds(10)), Is.EqualTo(BusHealthStatus.Degraded));

            Assert.That(await harness.Consumed.SelectAsync<MessageA>().Take(10).Count(), Is.EqualTo(10));

            Assert.That(await busControl.WaitForHealthStatus(BusHealthStatus.Healthy, TimeSpan.FromSeconds(10)), Is.EqualTo(BusHealthStatus.Healthy));
        });

        Assert.That(await harness.Consumed.SelectAsync<MessageA>().Take(20).Count(), Is.EqualTo(20));
    }


    public class MessageA
    {
    }


    public class MessageAReceived
    {
    }


    class MessageConsumer :
        IConsumer<MessageA>
    {
        public Task Consume(ConsumeContext<MessageA> context)
        {
            if (context.ReceiveContext.Redelivered)
                return context.Publish(new MessageAReceived());

            throw new IntentionalTestException("First time through, we want to explode");
        }
    }
}
