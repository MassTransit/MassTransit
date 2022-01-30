namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_specifying_a_condition_on_a_composite_event
    {
        [Test]
        public async Task Should_call_when_met()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            await _machine.RaiseEvent(_instance, Second);
            await _machine.RaiseEvent(_instance, First);

            Assert.IsTrue(_instance.Called);
            Assert.IsTrue(_instance.SecondFirst);
        }

        [Test]
        public async Task Should_skip_when_not_met()
        {
            _machine = CreateStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, Start);

            await _machine.RaiseEvent(_instance, First);
            await _machine.RaiseEvent(_instance, Second);

            Assert.IsFalse(_instance.Called);
            Assert.IsFalse(_instance.SecondFirst);
        }

        State Waiting;
        Event Start;
        Event First;
        Event Second;
        Event Third;

        StateMachine<Instance> _machine;
        Instance _instance;


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public CompositeEventStatus CompositeStatus { get; set; }
            public bool Called { get; set; }
            public bool CalledAfterAll { get; set; }
            public State CurrentState { get; set; }
            public bool SecondFirst { get; set; }
            public bool First { get; set; }
            public bool Second { get; set; }
        }

        private StateMachine<Instance> CreateStateMachine()
        {
            return MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Waiting", out Waiting)
                    .Event("Start", out Start)
                    .Event("First", out First)
                    .Event("Second", out Second)
                    .Initially()
                        .When(Start, b => b.TransitionTo(Waiting))
                    .During(Waiting)
                        .When(First, b => b.Then(context =>
                            {
                                context.Instance.First = true;
                                context.Instance.CalledAfterAll = false;
                            }))
                        .When(Second, b => b.Then(context =>
                            {
                                context.Instance.SecondFirst = !context.Instance.First;
                                context.Instance.Second = true;
                                context.Instance.CalledAfterAll = false;
                            }))
                    .CompositeEvent("Third", out Third, b => b.CompositeStatus, First, Second)
                    .During(Waiting)
                        .When(Third, context => context.Instance.SecondFirst, b => b
                            .Then(context =>
                            {
                                context.Instance.Called = true;
                                context.Instance.CalledAfterAll = true;
                            })
                            .Finalize()
                        )
                );
        }
    }
}
