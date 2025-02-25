namespace MassTransit.RabbitMqTransport.Tests;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Testing;


[TestFixture]
public class When_the_consumer_timeout_is_reached_waiting_for_a_batch
{
    [Test]
    [Explicit]
    public async Task Should_properly_handle_message_redelivery()
    {
        await using var provider = new ServiceCollection()
            .ConfigureRabbitMqTestOptions(options =>
            {
                options.CleanVirtualHost = true;
                options.CreateVirtualHostIfNotExists = true;
            })
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<RabbitMqTransportOptions>()
                    .Configure(options => options.VHost = "test");

                x.AddOptions<MassTransitHostOptions>().Configure(options =>
                {
                    options.StartTimeout = TimeSpan.FromSeconds(5);
                    options.StopTimeout = TimeSpan.FromSeconds(5);
                    options.ConsumerStopTimeout = TimeSpan.FromSeconds(1);
                });

                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(90), testTimeout: TimeSpan.FromSeconds(120));
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<HighTextMessageConsumer>(c => c.Options<BatchOptions>(options =>
                    {
                        options.TimeLimit = TimeSpan.FromSeconds(80);
                        options.MessageLimit = 10;
                    }))
                    .Endpoint(e => e.AddConfigureEndpointCallback(cfg =>
                    {
                        if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                            rmq.SetDeliveryAcknowledgementTimeout(ms: 10000);
                    }));

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider();

        var harness = await provider.StartTestHarness();

        await harness.Bus.PublishBatch(new TextMessage[]
        {
            new()
            {
                Text = "High Priority 1",
                Priority = "High"
            },
            new()
            {
                Text = "High Priority 2",
                Priority = "High"
            },
            new()
            {
                Text = "High Priority 3",
                Priority = "High"
            }
        });

        await harness.Bus.PublishBatch(new TextMessage[]
        {
            new()
            {
                Text = "High Priority 4",
                Priority = "High"
            },
            new()
            {
                Text = "High Priority 5",
                Priority = "High"
            },
            new()
            {
                Text = "High Priority 6",
                Priority = "High"
            }
        });

        Assert.That(await harness.Consumed.Any<TextMessage>(x => x.Exception == null));
    }


    public class TextMessage
    {
        public string Text { get; set; }
        public string Priority { get; set; }
    }


    class HighTextMessageConsumer :
        IConsumer<Batch<TextMessage>>
    {
        public async Task Consume(ConsumeContext<Batch<TextMessage>> context)
        {
            LogContext.Debug?.Log("Processing batch of {Count} messages", context.Message.Length);

            foreach (ConsumeContext<TextMessage> message in context.Message)
                LogContext.Debug?.Log("Got message: {0} {1} {2}", DateTime.UtcNow, message.Message.Text, message.Message.Priority);
        }
    }
}
