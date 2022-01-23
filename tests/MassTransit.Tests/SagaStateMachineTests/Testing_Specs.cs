namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_the_testing_framework_built_into_masstransit
    {
        [Test]
        public async Task Should_handle_the_initial_state()
        {
            var harness = new InMemoryTestHarness();
            ISagaStateMachineTestHarness<TestStateMachine, Instance> saga = harness.StateMachineSaga<Instance, TestStateMachine>(_machine);

            var sagaId = Guid.NewGuid();

            await harness.Start();
            try
            {
                await harness.InputQueueSendEndpoint.Send(new Start {CorrelationId = sagaId});

                Assert.IsTrue(harness.Consumed.Select<Start>().Any(), "Message not received");

                var instance = saga.Created.ContainsInState(sagaId, _machine, _machine.Running);
                Assert.IsNotNull(instance, "Saga instance not found");
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Test]
        public async Task Should_handle_the_stop_state()
        {
            var harness = new InMemoryTestHarness();
            ISagaStateMachineTestHarness<TestStateMachine, Instance> saga = harness.StateMachineSaga<Instance, TestStateMachine>(_machine);

            var sagaId = Guid.NewGuid();

            await harness.Start();
            try
            {
                await harness.InputQueueSendEndpoint.Send(new Start {CorrelationId = sagaId});

                Assert.IsTrue(harness.Consumed.Select<Start>().Any(), "Start not received");

                await harness.InputQueueSendEndpoint.Send(new Stop {CorrelationId = sagaId});

                Assert.IsTrue(harness.Consumed.Select<Stop>().Any(), "Stop not received");

                var instance = saga.Created.ContainsInState(sagaId, _machine, _machine.Final);
                Assert.IsNotNull(instance, "Saga instance not found");
            }
            finally
            {
                await harness.Stop();
            }
        }

        readonly TestStateMachine _machine;

        public Using_the_testing_framework_built_into_masstransit()
        {
            _machine = new TestStateMachine();
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
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started);
                Event(() => Stopped, x => x.CorrelateById(context => context.Message.CorrelationId));

                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Stop> Stopped { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
