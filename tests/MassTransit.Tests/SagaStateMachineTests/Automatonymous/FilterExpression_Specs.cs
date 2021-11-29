namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_specifying_a_conditional_event_activity
    {
        [Test]
        public async Task Should_transition_to_the_proper_state()
        {
            var instance = new Instance();
            var machine = new InstanceStateMachine();

            await machine.RaiseEvent(instance, machine.Thing, new Data {Condition = true});
            Assert.AreEqual(machine.True, instance.CurrentState);

            // reset
            instance.CurrentState = machine.Initial;

            await machine.RaiseEvent(instance, machine.Thing, new Data {Condition = false});
            Assert.AreEqual(machine.False, instance.CurrentState);
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                During(Initial,
                    When(Thing, context => context.Data.Condition)
                        .TransitionTo(True),
                    When(Thing, context => !context.Data.Condition)
                        .TransitionTo(False));
            }

            public State True { get; private set; }
            public State False { get; private set; }

            public Event<Data> Thing { get; private set; }
        }


        class Data
        {
            public bool Condition { get; set; }
        }
    }
}
