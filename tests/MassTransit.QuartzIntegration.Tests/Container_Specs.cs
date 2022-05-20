namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Quartz;
    using Scheduling;
    using Testing;


    [TestFixture]
    public class Using_the_container_setup_for_quartz
    {
        [Test]
        public async Task Should_have_an_even_cleaner_experience_without_owning_the_container()
        {
            await using var provider = new ServiceCollection()
                .AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddPublishMessageScheduler();

                    x.AddQuartzConsumers();

                    x.AddConsumer<FirstMessageConsumer>();
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
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(2);

            await harness.Start();

            await harness.Bus.Publish<FirstMessage>(new { });

            Assert.That(await harness.GetConsumerHarness<FirstMessageConsumer>().Consumed.Any<FirstMessage>(), Is.True);

            Assert.That(await harness.Consumed.Any<ScheduleMessage>(), Is.True);

            await adjustment.AdvanceTime(TimeSpan.FromSeconds(10));

            Assert.That(await harness.GetConsumerHarness<SecondMessageConsumer>().Consumed.Any<SecondMessage>(), Is.True);
        }


        public class FirstMessageConsumer :
            IConsumer<FirstMessage>
        {
            public async Task Consume(ConsumeContext<FirstMessage> context)
            {
                await context.SchedulePublish(TimeSpan.FromSeconds(10), new SecondMessage());
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
