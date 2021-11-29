namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Responding_through_the_outbox :
        InMemoryTestFixture
    {
        [Test]
        public void Should_receive_the_fault_message()
        {
            Assert.That(
                async () => await _client.GetResponse<StartupComplete>(new Start { FailToStart = true }, TestCancellationToken),
                Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_receive_the_response_message()
        {
            Response<StartupComplete> complete = await _client.GetResponse<StartupComplete>(new Start(), TestCancellationToken);
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _client = Bus.CreateRequestClient<Start>(InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.UseInMemoryOutbox();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;
        IRequestClient<Start> _client;


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Started, x => x.Data.FailToStart)
                        .Then(context => throw new IntentionalTestException()),
                    When(Started, x => x.Data.FailToStart == false)
                        .Respond(new StartupComplete())
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public bool FailToStart { get; set; }

            public Guid CorrelationId { get; private set; }
        }


        class StartupComplete
        {
        }
    }


    [TestFixture]
    public class When_a_fault_in_a_saga_machine_occurs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_fault_message_once()
        {
            var count = 0;
            Bus.ConnectHandler<Fault<Start>>(async context =>
            {
                Interlocked.Increment(ref count);
            });

            Assert.That(async () => await _client.GetResponse<StartupComplete>(new Start { FailToStart = true }, TestCancellationToken),
                Throws.TypeOf<RequestFaultException>());

            await InactivityTask;

            Assert.That(count, Is.EqualTo(1));
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _client = Bus.CreateRequestClient<Start>(InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.UseMessageRetry(r => r.Immediate(5));
            configurator.UseInMemoryOutbox();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;
        IRequestClient<Start> _client;


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Started, x => x.Data.FailToStart)
                        .Then(context => throw new IntentionalTestException()),
                    When(Started, x => x.Data.FailToStart == false)
                        .Respond(new StartupComplete())
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public bool FailToStart { get; set; }

            public Guid CorrelationId { get; private set; }
        }


        class StartupComplete
        {
        }
    }
}
