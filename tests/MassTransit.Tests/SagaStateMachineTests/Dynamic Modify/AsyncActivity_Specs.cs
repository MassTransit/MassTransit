namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Using_an_asynchronous_activity
    {
        [Test]
        public async Task Should_capture_the_value()
        {
            Event<CreateInstance> Create = null;

            var claim = new TestInstance();
            MassTransitStateMachine<TestInstance> machine = MassTransitStateMachine<TestInstance>
                .New(builder => builder
                    .State("Running", out State Running)
                    .Event("Create", out Create)
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                    .When(Create, b => b
                        .Execute(context => new SetValueAsyncActivity())
                        .TransitionTo(Running)
                    )
                );

            await machine.RaiseEvent(claim, Create, new CreateInstance());

            Assert.AreEqual("ExecuteAsync", claim.Value);
        }


        class TestInstance :
SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public string Value { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class SetValueAsyncActivity :
            IStateMachineActivity<TestInstance, CreateInstance>
        {
            public async Task Execute(BehaviorContext<TestInstance, CreateInstance> context,
                IBehavior<TestInstance, CreateInstance> next)
            {
                context.Instance.Value = "ExecuteAsync";
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TestInstance, CreateInstance, TException> context,
                IBehavior<TestInstance, CreateInstance> next)
                where TException : Exception
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


        class CreateInstance
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
