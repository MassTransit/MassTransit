namespace MassTransit.QuartzIntegration.Tests
{
    using System;
    using System.Linq;
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
                .AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
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

            Assert.That(await harness.GetConsumerHarness<FirstMessageConsumer>().Consumed.Any<FirstMessage>(), Is.True);

            Assert.That(await harness.Consumed.Any<ScheduleMessage>(), Is.True);

            await adjustment.AdvanceTime(TimeSpan.FromSeconds(10));

            Assert.That(await harness.GetConsumerHarness<SecondMessageConsumer>().Consumed.Any<SecondMessage>(), Is.True);
        }

        [Test]
        public async Task Should_work_properly_with_message_headers()
        {
            await using var provider = new ServiceCollection()
                .AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
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

            Assert.That(await harness.GetConsumerHarness<FirstMessageConsumer>().Consumed.Any<FirstMessage>(), Is.True);

            Assert.That(await harness.Consumed.Any<ScheduleMessage>(), Is.True);

            await adjustment.AdvanceTime(TimeSpan.FromSeconds(10));

            Assert.That(await harness.GetConsumerHarness<SecondMessageConsumer>().Consumed.Any<SecondMessage>(), Is.True);

            ConsumeContext<SecondMessage> context =
                (await harness.GetConsumerHarness<SecondMessageConsumer>().Consumed.SelectAsync<SecondMessage>().First()).Context;

            Assert.That(context.Headers.TryGetHeader("SimpleHeader", out var header), Is.True);

            Assert.That(header, Is.EqualTo("SimpleValue"));
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


    public class Using_with_saga_repository
    {
        [Test]
        public async Task Should_work_properly()
        {
            await using var provider = new ServiceCollection()
                .AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(3));

                    x.AddPublishMessageScheduler();

                    x.AddQuartzConsumers();

                    x.AddSagaStateMachine<ExampleStateMachine, ExampleStateMachineState>();

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UsePublishMessageScheduler();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            using var adjustment = new QuartzTimeAdjustment(provider);

            var harness = await provider.StartTestHarness();
            var sagaHarness = harness.GetSagaStateMachineHarness<ExampleStateMachine, ExampleStateMachineState>();

            var id = Guid.NewGuid();
            await harness.Bus.Publish<FirstMessage>(new
            {
                CorrelationId = id
            });
            try
            {
                Guid? existsId = await sagaHarness.Exists(id, machine => machine.PendingSecondMessage, TimeSpan.FromSeconds(10));

                Assert.That(existsId.HasValue, "saga does not exists");

                Assert.That((await sagaHarness.Exists(t => t.Timeouts == 0, machine => machine.PendingSecondMessage, TimeSpan.FromSeconds(2))).FirstOrDefault()
                    != default);

                await adjustment.AdvanceTime(TimeSpan.FromMinutes(10));
                Console.WriteLine("Advance 10 minutes");

                Assert.That((await sagaHarness.Exists(t => t.Timeouts == 1, machine => machine.PendingSecondMessage, TimeSpan.FromSeconds(2))).FirstOrDefault()
                    != default);

                await adjustment.AdvanceTime(TimeSpan.FromMinutes(10));

                Console.WriteLine("Advance 20 minutes in total");

                Assert.That((await sagaHarness.Exists(t => t.Timeouts == 2, machine => machine.PendingSecondMessage, TimeSpan.FromSeconds(2))).FirstOrDefault()
                    != default);

                await adjustment.AdvanceTime(TimeSpan.FromMinutes(10));

                Console.WriteLine("Advance 30 minutes in total");

                Assert.That((await sagaHarness.Exists(t => t.Timeouts == 3, machine => machine.PendingSecondMessage, TimeSpan.FromSeconds(2))).FirstOrDefault()
                    != default);

            }
            catch
            {
                await harness.OutputTimeline(TestContext.Out);
                throw;
            }

        }
        public class ExampleStateMachine : MassTransitStateMachine<ExampleStateMachineState>
        {
            public State PendingSecondMessage { get; set; }

            public ExampleStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Schedule(() => SecondMessageTimeout, x => x.Token, x => x.Delay = TimeSpan.FromMinutes(10));

                Initially(
                    When(FirstMessage)
                    .Schedule(SecondMessageTimeout, ctx => ctx.Init<SecondMessageTimeout>(new
                        {
                            ctx.Saga.CorrelationId
                        }))
                    .TransitionTo(PendingSecondMessage));

                During(PendingSecondMessage,
                        When(SecondMessageTimeoutEvent)
                            .Then(x => x.Saga.Timeouts += 1)
                            .IfElse(x => x.Saga.Timeouts < 3,
                                retry => retry.Schedule(SecondMessageTimeout, ctx =>
                                    ctx.Init<SecondMessageTimeout>(new
                                    {
                                        ctx.Saga.CorrelationId
                                    })
                                ),
                                reject => reject.Finalize())
                );
            }

            public Event<FirstMessage> FirstMessage { get; set; }
            public Event<SecondMessageTimeout> SecondMessageTimeoutEvent { get; set; }
            public Schedule<ExampleStateMachineState, SecondMessageTimeout> SecondMessageTimeout { get; set; } = default!;

        }
        public interface SecondMessageTimeout : CorrelatedBy<Guid>
        {
        }

        public interface FirstMessage : CorrelatedBy<Guid>
        {
        }

        public class ExampleStateMachineState : SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public string CurrentState { get; set; }
            public Guid? Token { get; set; }
            public int Timeouts { get; set; }
        }
    }
}
