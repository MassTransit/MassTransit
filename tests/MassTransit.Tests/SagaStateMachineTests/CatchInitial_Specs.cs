namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using CatchFault;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    namespace CatchFault
    {
        using System;
        using System.Threading.Tasks;
        using Microsoft.Extensions.Logging;


        public class SomeEvent
        {
            public Guid CorrelationId { get; set; }
        }


        public class SomeStateMachine :
            MassTransitStateMachine<SomeState>
        {
            public SomeStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(SomeEvent)
                        .Activity(x => x.OfType<FailureActivity>())
                        .TransitionTo(Active)
                        .Catch<Exception>(c => c
                            .Activity(x => x.OfType<BadThingHappenedActivity>())
                            .Finalize())
                );

                SetCompletedWhenFinalized();
            }

            public Event<SomeEvent> SomeEvent { get; }
            public State Active { get; }
        }


        public class SomeState :
            SagaStateMachineInstance
        {
            public string Something { get; set; }
            public string CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class BadThingHappenedActivity :
            IStateMachineActivity<SomeState, SomeEvent>
        {
            readonly ILogger<BadThingHappenedActivity> _logger;

            public BadThingHappenedActivity(ILogger<BadThingHappenedActivity> logger)
            {
                _logger = logger;
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public async Task Execute(BehaviorContext<SomeState, SomeEvent> context, IBehavior<SomeState, SomeEvent> next)
            {
                await next.Execute(context);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<SomeState, SomeEvent, TException> context, IBehavior<SomeState, SomeEvent> next)
                where TException : Exception
            {
                _logger?.LogError("Something bad happened");

                context.Saga.Something = "Caught";

                return next.Faulted(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("BadActivity");
            }
        }


        public class FailureActivity :
            IStateMachineActivity<SomeState, SomeEvent>
        {
            readonly ILogger<FailureActivity> _logger;

            public FailureActivity(ILogger<FailureActivity> logger)
            {
                _logger = logger;
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public async Task Execute(BehaviorContext<SomeState, SomeEvent> context, IBehavior<SomeState, SomeEvent> next)
            {
                _logger.LogWarning("Sorry, not sorry");

                throw new Exception("Randomness!");
            }

            public Task Faulted<TException>(BehaviorExceptionContext<SomeState, SomeEvent, TException> context, IBehavior<SomeState, SomeEvent> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("RandomFailure");
            }
        }
    }


    [TestFixture]
    public class When_an_activity_faults_in_initial
    {
        [Test]
        public async Task Should_discard_when_finalized()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddSagaStateMachine<SomeStateMachine, SomeState>();
                })
                .BuildServiceProvider(true);


            var harness = provider.GetTestHarness();
            harness.TestTimeout = TimeSpan.FromSeconds(3);

            await harness.Start();

            var id = NewId.NextGuid();

            await harness.Bus.Publish(new SomeEvent { CorrelationId = id });

            ISagaStateMachineTestHarness<SomeStateMachine, SomeState> sagaHarness = harness.GetSagaStateMachineHarness<SomeStateMachine, SomeState>();

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await sagaHarness.Consumed.Any<SomeEvent>(), Is.True);

                Assert.That(await sagaHarness.NotExists(id), Is.Null);
            });
        }
    }
}
