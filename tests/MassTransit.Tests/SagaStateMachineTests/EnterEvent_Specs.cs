namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Triggering_a_change_event_from_a_transition :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_a_double_state()
        {
            var sagaId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new Start {CorrelationId = sagaId});

            Guid? saga = await _repository.ShouldContainSagaInState(sagaId, _machine, _machine.RunningFaster, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            Assert.AreEqual(1, _repository[saga.Value].Instance.OnEnter);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public Triggering_a_change_event_from_a_transition()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
        }


        class Instance :
            SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public int Counter { get; set; }
            public int OnEnter { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started);
                Event(() => Stopped);

                Initially(
                    When(Started)
                        .Then(context =>
                        {
                            Console.WriteLine("Started:Then");
                            context.Instance.Counter = 1;
                        })
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());

                WhenEnter(Running, x => x.Then(context =>
                {
                    Console.WriteLine("Running.Enter:Then");
                    context.Instance.OnEnter = context.Instance.Counter;
                }).TransitionTo(RunningFaster));
            }

            public State Running { get; private set; }
            public State RunningFaster { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Stop> Stopped { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
