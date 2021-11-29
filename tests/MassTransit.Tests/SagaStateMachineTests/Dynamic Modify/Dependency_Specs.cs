namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using SagaStateMachine;


    [TestFixture(Category = "Dynamic Modify")]
    public class Having_a_dependency_available
    {
        [Test]
        public void Should_capture_the_value()
        {
            Assert.AreEqual("79", _claim.Value);
        }

        State Running;
        Event<CreateClaim> Create;
        ClaimAdjustmentInstance _claim;
        StateMachine<ClaimAdjustmentInstance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _claim = new ClaimAdjustmentInstance();
            _machine = MassTransitStateMachine<ClaimAdjustmentInstance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Create", out Create)
                    .InstanceState(x => x.CurrentState)
                    .During(builder.Initial)
                    .When(Create, b => b
                        .Execute(context => new CalculateValueActivity(new LocalCalculator()))
                        .Execute(context => new ActionActivity<ClaimAdjustmentInstance>(x =>
                        {
                        }))
                        .TransitionTo(Running)
                    )
                );

            var data = new CreateClaim
            {
                X = 56,
                Y = 23,
            };

            _machine.RaiseEvent(_claim, Create, data)
                .Wait();
        }


        class ClaimAdjustmentInstance :
            ClaimAdjustment,
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public string Value { get; set; }
        }


        class CalculateValueActivity :
            IStateMachineActivity<ClaimAdjustmentInstance, CreateClaim>
        {
            readonly CalculatorService _calculator;

            public CalculateValueActivity(CalculatorService calculator)
            {
                _calculator = calculator;
            }

            public async Task Execute(BehaviorContext<ClaimAdjustmentInstance, CreateClaim> context,
                IBehavior<ClaimAdjustmentInstance, CreateClaim> next)
            {
                context.Instance.Value = _calculator.Add(context.Data.X, context.Data.Y);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<ClaimAdjustmentInstance, CreateClaim, TException> context,
                IBehavior<ClaimAdjustmentInstance, CreateClaim> next)
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


        interface ClaimAdjustment :
            ClaimModel
        {
            State CurrentState { get; set; }
        }


        interface ClaimModel
        {
            string Value { get; set; }
        }


        class CreateClaim
        {
            public int X { get; set; }
            public int Y { get; set; }
        }


        interface CalculatorService
        {
            string Add(int x, int y);
        }


        class LocalCalculator :
            CalculatorService
        {
            public string Add(int x, int y)
            {
                return (x + y).ToString();
            }
        }
    }
}
