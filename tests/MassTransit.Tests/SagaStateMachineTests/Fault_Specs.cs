namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_an_activity_throws_an_exception :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_able_to_observe_its_own_event_fault()
        {
            var message = new Initialize();
            await InputQueueSendEndpoint.Send(message);

            Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, x => x.WaitingToStart, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await InputQueueSendEndpoint.Send(new Start(message.CorrelationId));

            saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, x => x.FailedToStart, TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        public async Task Should_be_received_as_a_fault_message()
        {
            var message = new Start();

            Task<ConsumeContext<Fault<Start>>> faultReceived =
                await ConnectPublishHandler<Fault<Start>>(x => message.CorrelationId == x.Message.Message.CorrelationId);

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<Fault<Start>> fault = await faultReceived;

            Assert.AreEqual(message.CorrelationId, fault.Message.Message.CorrelationId);
        }

        [Test]
        public async Task Should_observe_the_fault_message()
        {
            var message = new Initialize();

            Task<ConsumeContext<Fault<Start>>> faultReceived =
                await ConnectPublishHandler<Fault<Start>>(x => message.CorrelationId == x.Message.Message.CorrelationId);

            await InputQueueSendEndpoint.Send(message);

            Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, x => x.WaitingToStart, TestTimeout);

            await InputQueueSendEndpoint.Send(new Start(message.CorrelationId));

            ConsumeContext<Fault<Start>> fault = await faultReceived;

            Assert.AreEqual(message.CorrelationId, fault.Message.Message.CorrelationId);

            saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, x => x.FailedToStart, TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        public async Task Should_receive_a_fault_when_an_instance_does_not_exist()
        {
            var message = new Stop();

            Task<ConsumeContext<Fault<Stop>>> faultReceived =
                await ConnectPublishHandler<Fault<Stop>>(x => message.CorrelationId == x.Message.Message.CorrelationId);

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<Fault<Stop>> fault = await faultReceived;

            Assert.AreEqual(message.CorrelationId, fault.Message.Message.CorrelationId);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;


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
                InstanceState(x => x.CurrentState);

                Event(() => StartFaulted, x => x.CorrelateById(context => context.Message.Message.CorrelationId));
                Event(() => Stopped, x => x.OnMissingInstance(m => m.Fault()));

                Initially(
                    When(Started)
                        .Then(context =>
                        {
                            throw new NotSupportedException("This is expected, but nonetheless exceptional");
                        })
                        .TransitionTo(Running),
                    When(Initialized)
                        .TransitionTo(WaitingToStart),
                    When(Created)
                        .Then(context =>
                        {
                            throw new NotSupportedException("This is expected, but nonetheless exceptional");
                        })
                        .TransitionTo(Running));

                During(WaitingToStart,
                    When(Started)
                        .Then(instance =>
                        {
                            throw new NotSupportedException("This is expected, but nonetheless exceptional");
                        })
                        .TransitionTo(Running),
                    When(StartFaulted)
                        .TransitionTo(FailedToStart));

                During(Running,
                    When(Stopped)
                        .TransitionTo(Complete));
            }

            public State WaitingToStart { get; private set; }
            public State FailedToStart { get; private set; }
            public State Running { get; private set; }
            public State Complete { get; private set; }

            public Event<Start> Started { get; private set; }
            public Event<Initialize> Initialized { get; private set; }
            public Event<Create> Created { get; private set; }
            public Event<Fault<Start>> StartFaulted { get; private set; }
            public Event<Stop> Stopped { get; private set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Start(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        public class Stop :
            CorrelatedBy<Guid>
        {
            public Stop()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Stop(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        public class Initialize :
            CorrelatedBy<Guid>
        {
            public Initialize()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; set; }
        }


        public class Create :
            CorrelatedBy<Guid>
        {
            public Create()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; set; }
        }


        public class StartupComplete
        {
            public Guid TransactionId { get; set; }
        }
    }
}
