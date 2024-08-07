namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Tests;
    using MassTransit.Tests.Scenario;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Quartz;
    using Scheduling;
    using Testing;


    [TestFixture(typeof(Json))]
    [TestFixture(typeof(RawJson))]
    [TestFixture(typeof(NewtonsoftJson))]
    [TestFixture(typeof(NewtonsoftRawJson))]
    public class Using_quartz_with_serializer<T>
        where T : new()
    {
        [Test]
        public async Task Should_work_properly()
        {
            await using var provider = new ServiceCollection()
                .AddQuartz(_ => {
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(3));

                    x.AddPublishMessageScheduler();

                    x.AddQuartzConsumers();

                    x.AddConsumer<FirstMessageConsumer>();
                    x.AddConsumer<SecondMessageConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UsePublishMessageScheduler();

                        _configuration?.ConfigureBus(context, cfg);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            using var adjustment = new QuartzTimeAdjustment(provider);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish<FirstMessage>(new { });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.GetConsumerHarness<FirstMessageConsumer>().Consumed.Any<FirstMessage>(), Is.True);

                Assert.That(await harness.Consumed.Any<ScheduleMessage>(), Is.True);
            });

            await adjustment.AdvanceTime(TimeSpan.FromSeconds(10));

            Assert.That(await harness.GetConsumerHarness<SecondMessageConsumer>().Consumed.Any<SecondMessage>(), Is.True);
        }

        [Test]
        public async Task Should_work_properly_with_message_headers()
        {
            await using var provider = new ServiceCollection()
                .AddQuartz(_ =>
                {
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(3));

                    x.AddPublishMessageScheduler();

                    x.AddQuartzConsumers();

                    x.AddConsumer<FirstMessageConsumer>();
                    x.AddConsumer<SecondMessageConsumer>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UsePublishMessageScheduler();

                        _configuration?.ConfigureBus(context, cfg);

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            using var adjustment = new QuartzTimeAdjustment(provider);

            var harness = await provider.StartTestHarness();

            await harness.Bus.Publish<FirstMessage>(new { }, x => x.Headers.Set("SimpleHeader", "SimpleValue"));

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.GetConsumerHarness<FirstMessageConsumer>().Consumed.Any<FirstMessage>(), Is.True);

                Assert.That(await harness.Consumed.Any<ScheduleMessage>(), Is.True);
            });

            await adjustment.AdvanceTime(TimeSpan.FromSeconds(10));

            Assert.That(await harness.GetConsumerHarness<SecondMessageConsumer>().Consumed.Any<SecondMessage>(), Is.True);

            ConsumeContext<SecondMessage> context =
                (await harness.GetConsumerHarness<SecondMessageConsumer>().Consumed.SelectAsync<SecondMessage>().First()).Context;

            Assert.Multiple(() =>
            {
                Assert.That(context.Headers.TryGetHeader("SimpleHeader", out var header), Is.True);

                Assert.That(header, Is.EqualTo("SimpleValue"));
            });
        }

        readonly ITestBusConfiguration _configuration;

        public Using_quartz_with_serializer()
        {
            _configuration = new T() as ITestBusConfiguration;
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
