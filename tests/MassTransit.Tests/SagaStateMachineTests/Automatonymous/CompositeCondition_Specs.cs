namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_specifying_a_condition_on_a_composite_event
    {
        [Test]
        public async Task Should_call_when_met()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);
            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.Multiple(() =>
            {
                Assert.That(_instance.Called, Is.True);
                Assert.That(_instance.SecondFirst, Is.True);
            });
        }

        [Test]
        public async Task Should_skip_when_not_met()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.Multiple(() =>
            {
                Assert.That(_instance.Called, Is.False);
                Assert.That(_instance.SecondFirst, Is.False);
            });
        }

        TestStateMachine _machine;
        Instance _instance;


        class Instance :
            SagaStateMachineInstance
        {
            public CompositeEventStatus CompositeStatus { get; set; }
            public bool Called { get; set; }
            public bool CalledAfterAll { get; set; }
            public State CurrentState { get; set; }
            public bool SecondFirst { get; set; }
            public bool First { get; set; }
            public bool Second { get; set; }
            public Guid CorrelationId { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(First)
                        .Then(context =>
                        {
                            context.Instance.First = true;
                            context.Instance.CalledAfterAll = false;
                        }),
                    When(Second)
                        .Then(context =>
                        {
                            context.Instance.SecondFirst = !context.Instance.First;
                            context.Instance.Second = true;
                            context.Instance.CalledAfterAll = false;
                        })
                );

                CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

                During(Waiting,
                    When(Third, context => context.Instance.SecondFirst)
                        .Then(context =>
                        {
                            context.Instance.Called = true;
                            context.Instance.CalledAfterAll = true;
                        })
                        .Finalize());
            }

            public State Waiting { get; private set; }

            public Event Start { get; private set; }

            public Event First { get; private set; }
            public Event Second { get; private set; }
            public Event Third { get; private set; }
        }
    }
}
