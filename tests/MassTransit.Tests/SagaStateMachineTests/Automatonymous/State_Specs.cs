namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_state_is_declared
    {
        [Test]
        public void It_should_capture_the_name_of_final()
        {
            Assert.That(_machine.Final.Name, Is.EqualTo("Final"));
        }

        [Test]
        public void It_should_capture_the_name_of_initial()
        {
            Assert.That(_machine.Initial.Name, Is.EqualTo("Initial"));
        }

        [Test]
        public void It_should_capture_the_name_of_running()
        {
            Assert.That(_machine.Running.Name, Is.EqualTo("Running"));
        }

        [Test]
        public void Should_be_an_instance_of_the_proper_type()
        {
            Assert.That(_machine.Initial, Is.InstanceOf<MassTransitStateMachine<Instance>.StateMachineState>());
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
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
            Assert.That(_instance.CurrentState, Is.EqualTo("Running"));
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
            /// <summary>
            /// The CurrentState is exposed as a string for the ORM
            /// </summary>
            public string CurrentState { get; private set; }

            public Guid CorrelationId { get; set; }
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
            Assert.That(_machine.GetState(_instance).Result, Is.EqualTo(_machine.Running));
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
            /// <summary>
            /// The CurrentState is exposed as a string for the ORM
            /// </summary>
            public int CurrentState { get; private set; }

            public Guid CorrelationId { get; set; }
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
