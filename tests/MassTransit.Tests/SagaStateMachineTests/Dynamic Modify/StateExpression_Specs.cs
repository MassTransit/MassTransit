namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_creating_a_state_expression_for_int
    {
        [Test]
        public void It_should_match_the_state_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(Running);

            Func<Instance, bool> filter = expression.Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.True);
        }

        [Test]
        public void It_should_not_match_the_state_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(_machine.Initial);

            Func<Instance, bool> filter = expression.Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.False);
        }

        [Test]
        public void It_should_match_the_state_not_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(_machine.Initial);

            var filter = Expression.Lambda<Func<Instance, bool>>(Expression.Not(expression.Body), expression.Parameters).Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.True);
        }

        State Running;
        Event Started;
        StateMachine<Instance> _machine;
        Instance _instance;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Started", out Started)
                    .InstanceState(x => x.CurrentState, Running)
                    .Initially()
                        .When(Started, b => b.TransitionTo(Running))
                );
            _instance = new Instance();

            _machine.RaiseEvent(_instance, Started).Wait();
        }


        /// <summary>
        /// For this instance, the state is actually stored as a string. Therefore,
        /// it is important that the StateMachine property is initialized when the
        /// instance is hydrated, as it is used to get the State for the name of
        /// the current state. This makes it easier to save the instance using
        /// an ORM that doesn't support user types (cough, EF, cough).
        /// </summary>
        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            /// <summary>
            /// The CurrentState is exposed as a string for the ORM
            /// </summary>
            public int CurrentState { get; private set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class When_creating_a_state_expression_for_string
    {
        [Test]
        public void It_should_match_the_state_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(Running);

            Func<Instance, bool> filter = expression.Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.True);
        }

        [Test]
        public void It_should_not_match_the_state_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(_machine.Initial);

            Func<Instance, bool> filter = expression.Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.False);
        }

        [Test]
        public void It_should_match_the_state_not_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(_machine.Initial);

            var filter = Expression.Lambda<Func<Instance, bool>>(Expression.Not(expression.Body), expression.Parameters).Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.True);
        }

        State Running;
        Event Started;
        StateMachine<Instance> _machine;
        Instance _instance;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Started", out Started)
                    .InstanceState(x => x.CurrentState)
                    .Initially()
                        .When(Started, b => b.TransitionTo(Running))
                );
            _instance = new Instance();

            _machine.RaiseEvent(_instance, Started).Wait();
        }


        /// <summary>
        /// For this instance, the state is actually stored as a string. Therefore,
        /// it is important that the StateMachine property is initialized when the
        /// instance is hydrated, as it is used to get the State for the name of
        /// the current state. This makes it easier to save the instance using
        /// an ORM that doesn't support user types (cough, EF, cough).
        /// </summary>
        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            /// <summary>
            /// The CurrentState is exposed as a string for the ORM
            /// </summary>
            public string CurrentState { get; private set; }
        }
    }

    [TestFixture(Category = "Dynamic Modify")]
    public class When_creating_a_state_expression_for_raw
    {
        [Test]
        public void It_should_match_the_state_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(Running);

            Func<Instance, bool> filter = expression.Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.True);
        }

        [Test]
        public void It_should_not_match_the_state_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(_machine.Initial);

            Func<Instance, bool> filter = expression.Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.False);
        }

        [Test]
        public void It_should_match_the_state_not_requested()
        {
            var expression = ((StateMachine<Instance>)_machine).Accessor.GetStateExpression(_machine.Initial);

            var filter = Expression.Lambda<Func<Instance, bool>>(Expression.Not(expression.Body), expression.Parameters).Compile();

            bool result = filter(_instance);

            Assert.That(result, Is.True);
        }

        State Running;
        Event Started;
        StateMachine<Instance> _machine;
        Instance _instance;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Started", out Started)
                    .InstanceState(x => x.CurrentState)
                    .Initially()
                        .When(Started, b => b.TransitionTo(Running))
                );
            _instance = new Instance();

            _machine.RaiseEvent(_instance, Started).Wait();
        }


        /// <summary>
        /// For this instance, the state is actually stored as a string. Therefore,
        /// it is important that the StateMachine property is initialized when the
        /// instance is hydrated, as it is used to get the State for the name of
        /// the current state. This makes it easier to save the instance using
        /// an ORM that doesn't support user types (cough, EF, cough).
        /// </summary>
        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            /// <summary>
            /// The CurrentState is exposed as a string for the ORM
            /// </summary>
            public State CurrentState { get; private set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .TransitionTo(Running));
            }

            public Event Started { get; private set; }
            public State Running { get; private set; }
        }
    }
}
