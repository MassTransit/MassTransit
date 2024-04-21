namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using DelaySubjects;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Testing;


    [TestFixture]
    public class DelayDirectExchange_Specs
    {
        [Test]
        public async Task Should_properly_deliver_the_message()
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

                    x.AddDelayedMessageScheduler();

                    x.AddConsumer<MyLowTextMessageConsumer>();
                    x.AddConsumer<MyHighTextMessageConsumer>();

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(3));

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.UseDelayedMessageScheduler();

                        cfg.Send<TextMessage>(c =>
                        {
                            c.UseRoutingKeyFormatter(msg => msg.Message.Priority);
                        });

                        cfg.Publish<TextMessage>(c =>
                        {
                            c.ExchangeType = ExchangeType.Direct;
                        });

                        cfg.ReceiveEndpoint($"{nameof(TextMessage)}_Low", e =>
                        {
                            e.ConfigureConsumeTopology = false;
                            e.Bind<TextMessage>(s =>
                            {
                                s.RoutingKey = "Low";
                                s.ExchangeType = ExchangeType.Direct;
                            });

                            e.ConfigureConsumer<MyLowTextMessageConsumer>(context);
                        });

                        cfg.ReceiveEndpoint($"{nameof(TextMessage)}_High", e =>
                        {
                            e.ConfigureConsumeTopology = false;
                            e.Bind<TextMessage>(s =>
                            {
                                s.RoutingKey = "High";
                                s.ExchangeType = ExchangeType.Direct;
                            });

                            e.ConfigureConsumer<MyHighTextMessageConsumer>(context);
                        });
                    });
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish(new TextMessage
            {
                Text = "High Priority",
                Priority = "High"
            });

            await harness.Bus.Publish(new TextMessage
            {
                Text = "Low Priority 1",
                Priority = "Low"
            });

            var scheduler = harness.Scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

            await scheduler.SchedulePublish(DateTime.UtcNow.AddSeconds(1), new TextMessage
            {
                Text = "Low Priority 2",
                Priority = "Low"
            });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Consumed.Any<TextMessage>(x => x.Context.Message.Text == "High Priority"), Is.True, "High");
                Assert.That(await harness.Consumed.Any<TextMessage>(x => x.Context.Message.Text == "Low Priority 1"), Is.True, "Low 1");
                Assert.That(await harness.Consumed.Any<TextMessage>(x => x.Context.Message.Text == "Low Priority 2"), Is.True, "Low 2");
            });
        }
    }


    namespace DelaySubjects
    {
        class MyHighTextMessageConsumer :
            IConsumer<TextMessage>
        {
            public async Task Consume(ConsumeContext<TextMessage> context)
            {
            }
        }


        class MyLowTextMessageConsumer :
            IConsumer<TextMessage>
        {
            public async Task Consume(ConsumeContext<TextMessage> context)
            {
            }
        }


        public class TextMessage
        {
            public string Text { get; set; }
            public string Priority { get; set; }
        }
    }
}
