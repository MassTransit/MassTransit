namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_specifying_a_conditional_event_activity
    {
        [Test]
        public async Task Should_transition_to_the_proper_state()
        {
            State True = null;
            State False = null;
            Event<Data> Thing = null;

            var instance = new Instance();
            var machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("True", out True)
                    .State("False", out False)
                    .Event("Thing", out Thing)
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                        .When(Thing, context => context.Data.Condition, b => b.TransitionTo(True))
                        .When(Thing, context => !context.Data.Condition, b => b.TransitionTo(False))
                );

            await machine.RaiseEvent(instance, Thing, new Data {Condition = true});
            Assert.AreEqual(True, instance.CurrentState);

            // reset
            instance.CurrentState = machine.Initial;

            await machine.RaiseEvent(instance, Thing, new Data {Condition = false});
            Assert.AreEqual(False, instance.CurrentState);
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
