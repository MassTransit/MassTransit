namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Quartz;
    using QuartzIntegration;
    using Scheduling;
    using Testing;


    [TestFixture]
    public class Using_the_quartz_scheduler_with_the_outbox
    {
        [Test]
        public async Task Should_delay_message_scheduling_until_the_outbox_messages_are_delivered()
        {
            await using var provider = new ServiceCollection()
                .AddQuartz(_ =>
                {
                })
                .AddBusOutboxServices()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddPublishMessageScheduler();

                    x.AddEntityFrameworkOutbox<ReliableDbContext>(o =>
                    {
                        o.QueryDelay = TimeSpan.FromSeconds(1);
                    });

                    x.AddQuartzConsumers();

                    x.AddConsumer<FirstMessageConsumer, FirstMessageConsumerDefinition>();
                    x.AddConsumer<SecondMessageConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UsePublishMessageScheduler();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            using var adjustment = new QuartzTimeAdjustment(provider);

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            try
            {
                await harness.Bus.Publish<FirstMessage>(new { });

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(await harness.GetConsumerHarness<FirstMessageConsumer>().Consumed.Any<FirstMessage>(), Is.True);

                    Assert.That(await harness.Consumed.Any<ScheduleMessage>(), Is.True);
                });

                await adjustment.AdvanceTime(TimeSpan.FromSeconds(10));

                Assert.That(await harness.GetConsumerHarness<SecondMessageConsumer>().Consumed.Any<SecondMessage>(), Is.True);
            }
            finally
            {
                await harness.Stop();
            }
        }


        public class FirstMessageConsumer :
            IConsumer<FirstMessage>
        {
            public async Task Consume(ConsumeContext<FirstMessage> context)
            {
                await context.SchedulePublish(TimeSpan.FromSeconds(10), new SecondMessage());
            }
        }


        public class FirstMessageConsumerDefinition :
            ConsumerDefinition<FirstMessageConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<FirstMessageConsumer> consumerConfigurator, IRegistrationContext context)
            {
                endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 100, 100));

                endpointConfigurator.UseEntityFrameworkOutbox<ReliableDbContext>(context);
            }
        }


        public class SecondMessageConsumer :
            IConsumer<SecondMessage>
        {
            public Task Consume(ConsumeContext<SecondMessage> context)
            {
                return Task.CompletedTask;
            }
        }


        public class FirstMessage
        {
        }


        public class SecondMessage
        {
        }
    }
}
