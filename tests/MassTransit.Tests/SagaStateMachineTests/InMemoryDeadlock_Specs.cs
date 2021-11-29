namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class InMemoryDeadlock_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_deadlock_on_the_repository()
        {
            var id = NewId.NextGuid();

            await InputQueueSendEndpoint.Send<CreateInstance>(new {CorrelationId = id});

            Guid? saga = await _repository.ShouldContainSagaInState(id, _machine, _machine.Active, TestTimeout);

            Assert.IsTrue(saga.HasValue);

            await InputQueueSendEndpoint.Send<CompleteInstance>(new {CorrelationId = id});

            await Task.Delay(990);

            await Console.Out.WriteLineAsync("Sending duplicate message");

            await InputQueueSendEndpoint.Send<CancelInstance>(new {CorrelationId = id});

            id = NewId.NextGuid();
            await InputQueueSendEndpoint.Send<CreateInstance>(new {CorrelationId = id});
            Guid? saga2 = await _repository.ShouldContainSagaInState(id, _machine, _machine.Active, TestTimeout);
            Assert.IsTrue(saga2.HasValue);
        }

        InMemorySagaRepository<Instance> _repository;
        DeadlockStateMachine _machine;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _repository = new InMemorySagaRepository<Instance>();
            _machine = new DeadlockStateMachine();

            configurator.StateMachineSaga(_machine, _repository);
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class DeadlockStateMachine :
            MassTransitStateMachine<Instance>
        {
            public DeadlockStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Create)
                        .TransitionTo(Active));

                During(Active,
                    When(Complete)
                        .ThenAsync(async context =>
                        {
                            await Console.Out.WriteLineAsync($"Completing: {context.Instance.CorrelationId}");
                            await Task.Delay(1000);
                            await Console.Out.WriteLineAsync($"Completed: {context.Instance.CorrelationId}");
                        })
                        .Finalize(),
                    When(Cancel)
                        .ThenAsync(async context =>
                        {
                            await Console.Out.WriteLineAsync($"Canceling: {context.Instance.CorrelationId}");
                            await Task.Delay(1000);
                            await Console.Out.WriteLineAsync($"Canceled: {context.Instance.CorrelationId}");
                        })
                        .Finalize());

                SetCompletedWhenFinalized();
            }

            public State Active { get; private set; }
            public Event<CreateInstance> Create { get; private set; }
            public Event<CompleteInstance> Complete { get; private set; }
            public Event<CancelInstance> Cancel { get; private set; }
        }


        public interface CreateInstance :
            CorrelatedBy<Guid>
        {
        }


        public interface CompleteInstance :
            CorrelatedBy<Guid>
        {
        }


        public interface CancelInstance :
            CorrelatedBy<Guid>
        {
        }
    }
}
