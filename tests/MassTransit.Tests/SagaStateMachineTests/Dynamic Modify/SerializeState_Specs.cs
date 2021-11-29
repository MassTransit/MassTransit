namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Serializing_a_state_instance
    {
        [Test]
        public async Task Should_properly_handle_the_state_property()
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
                    .During(builder.Initial)
                        .When(Thing, context => context.Data.Condition, b => b.TransitionTo(True))
                        .When(Thing, context => !context.Data.Condition, b => b.TransitionTo(False))
                );

            await machine.RaiseEvent(instance, Thing, new Data
            {
                Condition = true
            });
            Assert.AreEqual(True, instance.CurrentState);

            var serializer = new JsonStateSerializer<StateMachine<Instance>, Instance>(machine);

            string body = serializer.Serialize(instance);

            Console.WriteLine("Body: {0}", body);
            var reInstance = serializer.Deserialize<Instance>(body);

            Assert.AreEqual(True, reInstance.CurrentState);
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
