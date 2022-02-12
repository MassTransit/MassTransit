namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Responding_from_within_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public void Should_fault_on_a_missing_instance()
        {
            Assert.That(
                async () => await _statusClient.GetResponse<StatusReport>(new StatusRequested(NewId.NextGuid()), TestCancellationToken),
                Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_receive_the_response_message()
        {
            Response<StartupComplete> complete = await _client.GetResponse<StartupComplete>(new Start(), TestCancellationToken);
        }

        [Test]
        public async Task Should_start_and_report_status()
        {
            var start = new Start();

            Response<StartupComplete> complete = await _client.GetResponse<StartupComplete>(start, TestCancellationToken);

            Response<StatusReport> response = await _statusClient.GetResponse<StatusReport>(new StatusRequested(start.CorrelationId), TestCancellationToken);

            response.Message.Status.ShouldBe(_machine.Running.Name);
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _client = Bus.CreateRequestClient<Start>(InputQueueAddress, TestTimeout);
            _statusClient = Bus.CreateRequestClient<StatusRequested>(InputQueueAddress, TestTimeout);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;
        IRequestClient<Start> _client;
        IRequestClient<StatusRequested> _statusClient;


        class Instance :
            SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Event(() => Requested, x => x.OnMissingInstance(m => m.Fault()));

                Initially(
                    When(Started)
                        .Respond(new StartupComplete())
                        .TransitionTo(Running));

                DuringAny(
                    When(Requested)
                        .RespondAsync(x => x.Init<StatusReport>(new
                        {
                            x.Instance.CorrelationId,
                            Status = x.Instance.CurrentState
                        })));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<StatusRequested> Requested { get; private set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; set; }
        }


        public class StatusRequested :
            CorrelatedBy<Guid>
        {
            public StatusRequested(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        class StatusReport :
            CorrelatedBy<Guid>
        {
            public string Status { get; set; }

            public Guid CorrelationId { get; set; }
        }


        class StartupComplete
        {
        }
    }
}
