namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_combining_events_into_a_single_event_in_the_initial_state :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_combined_event()
        {
            var correlationId = Guid.NewGuid();
            var firstMessage = new FirstMessage(correlationId);
            var secondMessage = new SecondMessage(correlationId);

            await InputQueueSendEndpoint.Send(firstMessage);
            await InputQueueSendEndpoint.Send(secondMessage);

            Guid? saga = await _repository.ShouldContainSaga(x => x.CorrelationId == correlationId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            Task<ConsumeContext<CompleteMessage>> received =
                await ConnectPublishHandler<CompleteMessage>(x => x.Message.CorrelationId == correlationId);

            await received;
        }

        InMemorySagaRepository<Instance> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;


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

            public int CompositeStatus { get; set; }
            public int CurrentState { get; set; }

            public bool ReceivedFirst { get; set; }
            public bool ReceivedSecond { get; set; }

            public Guid CorrelationId { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => First, x => x.CorrelateById(m => m.Message.CorrelationId));
                Event(() => Second, x => x.CorrelateById(m => m.Message.CorrelationId));

                Initially(
                    When(First)
                        .Then(ctx =>
                        {
                            ctx.Instance.ReceivedFirst = true;
                        }),
                    When(Second)
                        .Then(ctx =>
                        {
                            ctx.Instance.ReceivedSecond = true;
                        }));

                CompositeEvent(
                    () => Third,
                    x => x.CompositeStatus,
                    CompositeEventOptions.IncludeInitial,
                    First, Second);

                Initially(
                    When(Third)
                        .Publish(ctx => new CompleteMessage(ctx.Instance.CorrelationId))
                );
            }

            public Event<FirstMessage> First { get; private set; }
            public Event<SecondMessage> Second { get; private set; }
            public Event Third { get; private set; }
        }


        class FirstMessage :
            CorrelatedBy<Guid>
        {
            public FirstMessage(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        class SecondMessage
        {
            public SecondMessage(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        class CompleteMessage :
            CorrelatedBy<Guid>
        {
            public CompleteMessage(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }
    }
}
