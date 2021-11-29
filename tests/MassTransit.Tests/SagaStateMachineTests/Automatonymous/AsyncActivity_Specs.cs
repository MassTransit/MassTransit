namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Using_an_asynchronous_activity
    {
        [Test]
        public async Task Should_capture_the_value()
        {
            var claim = new TestInstance();
            var machine = new TestStateMachine();

            await machine.RaiseEvent(claim, machine.Create, new CreateInstance());

            Assert.AreEqual("ExecuteAsync", claim.Value);
        }


        class TestInstance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }


        class SetValueAsyncActivity :
            IStateMachineActivity<TestInstance, CreateInstance>
        {
            async Task IStateMachineActivity<TestInstance, CreateInstance>.Execute(BehaviorContext<TestInstance, CreateInstance> context,
                IBehavior<TestInstance, CreateInstance> next)
            {
                context.Instance.Value = "ExecuteAsync";
            }

            Task IStateMachineActivity<TestInstance, CreateInstance>.Faulted<TException>(BehaviorExceptionContext<TestInstance, CreateInstance, TException> context,
                IBehavior<TestInstance, CreateInstance> next)
            {
                return next.Faulted(context);
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public void Probe(ProbeContext context)
            {
            }
        }


        class CreateInstance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<TestInstance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                During(Initial,
                    When(Create)
                        .Execute(context => new SetValueAsyncActivity())
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event<CreateInstance> Create { get; private set; }
        }
    }
}
