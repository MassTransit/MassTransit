namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;
    using SagaStateMachine;


    [TestFixture]
    public class When_a_state_is_declared
    {
        [Test]
        public void It_should_capture_the_name_of_final()
        {
            Assert.AreEqual("Final", _machine.Final.Name);
        }

        [Test]
        public void It_should_capture_the_name_of_initial()
        {
            Assert.AreEqual("Initial", _machine.Initial.Name);
        }

        [Test]
        public void It_should_capture_the_name_of_running()
        {
            Assert.AreEqual("Running", _machine.Running.Name);
        }

        [Test]
        public void Should_be_an_instance_of_the_proper_type()
        {
            Assert.IsInstanceOf<StateMachineState<Instance>>(_machine.Initial);
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
        }


        TestStateMachine _machine;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);
            }

            public State Running { get; private set; }
        }
    }


    [TestFixture]
    public class When_a_state_is_stored_another_way
    {
        [Test]
        public void It_should_get_the_name_right()
        {
            Assert.AreEqual("Running", _instance.CurrentState);
        }

        TestStateMachine _machine;
        Instance _instance;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();

            _machine.RaiseEvent(_instance, x => x.Started).Wait();
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


    [TestFixture]
    public class When_storing_state_as_an_int
    {
        [Test]
        public void It_should_get_the_name_right()
        {
            Assert.AreEqual(_machine.Running, _machine.GetState(_instance).Result);
        }

        TestStateMachine _machine;
        Instance _instance;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();

            _machine.RaiseEvent(_instance, x => x.Started).Wait();
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


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState, Running);

                Initially(
                    When(Started)
                        .TransitionTo(Running));
            }

            public Event Started { get; private set; }
            public State Running { get; private set; }
        }
    }
}
