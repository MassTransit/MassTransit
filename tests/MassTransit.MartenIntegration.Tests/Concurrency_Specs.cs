namespace MassTransit.MartenIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using ConcurrentSagaTypes;
    using Marten;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class When_consuming_concurrent_messages_for_the_same_saga_instance
    {
        [Test]
        public async Task Should_properly_transition_the_state()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(5));

                    x.AddHandler(async (ConsumeContext<Running> context) =>
                    {
                        await Task.WhenAll(
                            context.Publish(new RunningA(context.Message.CorrelationId)),
                            context.Publish(new RunningB(context.Message.CorrelationId)));
                    });

                    x.AddHandler(async (ConsumeContext<Completing> context) =>
                    {
                        await Task.WhenAll(
                            context.Publish(new CompletingA(context.Message.CorrelationId)),
                            context.Publish(new CompletingB(context.Message.CorrelationId)));
                    });

                    x.AddMarten().ApplyAllDatabaseChangesOnStartup();

                    x.AddSagaStateMachine<TransactionStateMachine, TransactionState>()
                        .MartenRepository("server=localhost;port=5432;database=MartenTest;user id=postgres;password=Password12!;", r =>
                        {
                            r.CreateDatabasesForTenants(c =>
                            {
                                c.MaintenanceDatabase("server=localhost;port=5432;database=postgres;user id=postgres;password=Password12!;");
                                c.ForTenant()
                                    .CheckAgainstPgDatabase()
                                    .WithOwner("postgres")
                                    .WithEncoding("UTF-8")
                                    .ConnectionLimit(-1);
                            });
                        });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseMessageRetry(r => r.Immediate(5));
                        cfg.UseMessageScope(context);
                        cfg.UseInMemoryOutbox();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            Guid correlationId = NewId.NextGuid();

            await harness.Bus.Publish(new Started(correlationId));

            Assert.That(await harness.Published.Any<Completed>(x => x.Context.Message.CorrelationId == correlationId));
        }
    }


    namespace ConcurrentSagaTypes
    {
        using System;


        public record Started(Guid CorrelationId);


        public record Running(Guid CorrelationId);


        public record RunningA(Guid CorrelationId);


        public record RunningB(Guid CorrelationId);


        public record Completing(Guid CorrelationId);


        public record CompletingA(Guid CorrelationId);


        public record CompletingB(Guid CorrelationId);


        public record Completed(Guid CorrelationId);


        public class TransactionState :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public int State { get; set; }

            public int RunningStatus { get; set; }
            public int CompletingStatus { get; set; }
        }


        public class TransactionStateMachine :
            MassTransitStateMachine<TransactionState>
        {
            public TransactionStateMachine()
            {
                InstanceState(x => x.State);

                Initially(
                    When(Started)
                        .TransitionTo(Running)
                        .Publish(context => new Running(context.Message.CorrelationId))
                );

                During(Running,
                    When(RunningFinished)
                        .TransitionTo(Completing)
                        .Publish(context => new Completing(context.Saga.CorrelationId))
                );

                During(Completing,
                    When(CompletingFinished)
                        .TransitionTo(Completed)
                        .Publish(context => new Completed(context.Saga.CorrelationId))
                );

                CompositeEvent(() => RunningFinished, x => x.RunningStatus, RunningA, RunningB);
                CompositeEvent(() => CompletingFinished, x => x.CompletingStatus, CompletingA, CompletingB);

                SetCompletedWhenFinalized();
            }

            // ReSharper disable UnassignedGetOnlyAutoProperty
            public State Running { get; }
            public State Completing { get; }
            public State Completed { get; }

            public Event<Started> Started { get; }
            public Event<RunningA> RunningA { get; }
            public Event<RunningB> RunningB { get; }
            public Event RunningFinished { get; }
            public Event<CompletingA> CompletingA { get; }
            public Event<CompletingB> CompletingB { get; }
            public Event CompletingFinished { get; }
        }
    }
}
