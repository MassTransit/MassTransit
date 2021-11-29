namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using NUnit.Framework;
    using SagaStateMachine;


    [TestFixture(Category = "Dynamic Modify")]
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
            Assert.AreEqual("Running", Running.Name);
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

        State Running;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .InstanceState(x => x.CurrentState)
                );
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class When_a_state_is_stored_another_way
    {
        [Test]
        public void It_should_get_the_name_right()
        {
            Assert.AreEqual("Running", _instance.CurrentState);
        }

        Event Started;
        StateMachine<Instance> _machine;
        Instance _instance;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .Event("Started", out Started)
                    .State("Running", out State Running)
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
    public class When_storing_state_as_an_int
    {
        [Test]
        public void It_should_get_the_name_right()
        {
            Assert.AreEqual(Running, _machine.GetState(_instance).Result);
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
}
