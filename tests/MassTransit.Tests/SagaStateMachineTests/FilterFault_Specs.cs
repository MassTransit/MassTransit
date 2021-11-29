namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_an_exception_is_filtered :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_able_to_observe_its_own_event_fault()
        {
            Task<ConsumeContext<Fault<Start>>> faulted = await ConnectPublishHandler<Fault<Start>>();

            var message = new Initialize();
            await InputQueueSendEndpoint.Send(message);

            Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, _machine.WaitingToStart, TimeSpan.FromSeconds(8));
            Assert.IsTrue(saga.HasValue);

            await InputQueueSendEndpoint.Send(new Start(message.CorrelationId));

            await faulted;

            saga = await _repository.ShouldContainSagaInState(x => x.CorrelationId == message.CorrelationId && x.StartAttempts == 1, _machine,
                _machine.WaitingToStart, TimeSpan.FromSeconds(8));
            Assert.IsTrue(saga.HasValue);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.UseRetry(x =>
            {
                x.Ignore<NotSupportedException>();
                x.Immediate(2);
            });

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

            public int StartAttempts { get; set; }
            public State CurrentState { get; set; }
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
                        .Then(context =>
                        {
                            context.Instance.StartAttempts++;
                            throw new NotSupportedException("This is expected, but nonetheless exceptional");
                        })
                        .TransitionTo(Running),
                    When(Initialized)
                        .TransitionTo(WaitingToStart));

                During(WaitingToStart,
                    When(Started)
                        .Then(instance =>
                        {
                            instance.Instance.StartAttempts++;
                            throw new NotSupportedException("This is expected, but nonetheless exceptional");
                        })
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .TransitionTo(Complete));
            }

            public State WaitingToStart { get; private set; }
            public State Running { get; private set; }
            public State Complete { get; private set; }

            public Event<Start> Started { get; private set; }
            public Event<Initialize> Initialized { get; private set; }
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
    }
}
