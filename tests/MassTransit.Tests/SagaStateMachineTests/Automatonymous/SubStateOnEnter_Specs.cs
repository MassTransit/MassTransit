namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class SubStateOnEnter_Specs
    {
        [Test]
        public async Task Should_raise_both_enter_events()
        {
            var instance = new StateData();
            var machine = new StateMachine();
            var observer = new StateChangeObserver<StateData>();
            var eventObserver = new EventRaisedObserver<StateData>();

            using var subscription = machine.ConnectStateObserver(observer);
            using var beforeEnterSub = machine.ConnectEventObserver(machine.s2.Enter, eventObserver);

            await machine.RaiseEvent(instance, machine.Start); // go to s1
            await machine.RaiseEvent(instance, machine.Boom); // go to s2
            await machine.RaiseEvent(instance, machine.ToSub); // s2 does not handle ToSub
            await machine.RaiseEvent(instance, machine.Boom); // go to s1
            await machine.RaiseEvent(instance, machine.ToSub); // go to s21 --> Enter s2 is missing here!
            await machine.RaiseEvent(instance, machine.Quit);

            Assert.That(eventObserver.Events.Count, Is.EqualTo(2));
        }


        public class StateData :
            SagaStateMachineInstance
        {
            public string CurrentSate { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class StateMachine :
            MassTransitStateMachine<StateData>
        {
            public StateMachine()
            {
                InstanceState(x => x.CurrentSate);

                SubState(() => s21, s2);

                Initially(When(Start).TransitionTo(s1));

                WhenEnterAny(x => x.Then(context => Console.WriteLine($"Enter {context.Instance.CurrentSate} ({context.Event.Name}).")));
                WhenLeaveAny(x => x.Then(context => Console.WriteLine($"Leave {context.Instance.CurrentSate} ({context.Event.Name}).")));

                During(s1,
                    When(Boom).TransitionTo(s2),
                    When(ToSub).TransitionTo(s21)
                );
                During(s2,
                    When(Boom).TransitionTo(s1)
                );

                DuringAny(When(Quit).Finalize());

                Finally(x => x.Then(context => Console.WriteLine("We're done.")));

                OnUnhandledEvent(context => Console.Out.WriteLineAsync($"{context.Instance.CurrentSate} does not handle {context.Event}!"));
            }

            public State s1 { get; private set; }
            public State s2 { get; private set; }
            public State s21 { get; private set; }

            public Event Start { get; private set; }
            public Event Boom { get; private set; }
            public Event ToSub { get; private set; }
            public Event Quit { get; private set; }
        }
    }
}
