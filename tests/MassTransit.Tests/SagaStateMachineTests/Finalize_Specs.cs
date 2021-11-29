namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    public class Finalize_Specs
        : InMemoryTestFixture
    {
        readonly TaskCompletionSource<bool> _taskCompletionSource = new TaskCompletionSource<bool>();
        TestStateMachine _machine;

        InMemorySagaRepository<Instance> _repository;

        [Test]
        public async Task Should_remove_saga_when_completed_in_whenenter()
        {
            var correlationId = Guid.NewGuid();
            var firstMessage = new FirstMessage(correlationId);

            await InputQueueSendEndpoint.Send(firstMessage);

            Guid? saga = await _repository.ShouldContainSagaInState(correlationId, _machine, x => x.OtherState, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            _taskCompletionSource.SetResult(true);

            saga = await _repository.ShouldNotContainSaga(correlationId, TestTimeout);
            Assert.IsFalse(saga.HasValue);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine(_taskCompletionSource.Task);
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }


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

            public int CurrentState { get; set; }

            public bool ReceivedFirst { get; set; }
            public bool TruthProvided { get; set; }

            public Guid CorrelationId { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine(Task<bool> truthProvider)
            {
                InstanceState(x => x.CurrentState);

                Event(() => First, x => x.CorrelateById(m => m.Message.CorrelationId));

                SetCompletedWhenFinalized();

                Initially(
                    When(First)
                        .Then(ctx =>
                        {
                            ctx.Instance.ReceivedFirst = true;
                        }).TransitionTo(OtherState)
                );

                WhenEnter(OtherState, x => x
                    .ThenAsync(async ctx =>
                    {
                        ctx.Instance.TruthProvided = await truthProvider;
                    })
                    .If(ctx => ctx.Instance.ReceivedFirst && ctx.Instance.TruthProvided,
                        ctx => ctx.Finalize())
                );
            }

            public State OtherState { get; private set; }

            public Event<FirstMessage> First { get; private set; }
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
    }
}
