namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Configuring_a_message_in_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_all_the_stuff()
        {
            await InputQueueSendEndpoint.Send(new Start());

            await _message.Task;

            await _consumerOnly.Task;

            await _consumerMessage.Task;
        }

        TaskCompletionSource<Start> _message;
        TaskCompletionSource<Tuple<Instance, Start>> _consumerMessage;
        TaskCompletionSource<Instance> _consumerOnly;
        InMemorySagaRepository<Instance> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _message = GetTask<Start>();
            _consumerOnly = GetTask<Instance>();
            _consumerMessage = GetTask<Tuple<Instance, Start>>();

            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(new TestStateMachine(), _repository, cfg =>
            {
                cfg.UseExecute(context => _consumerOnly.TrySetResult(context.Saga));
                cfg.Message<Start>(m => m.UseExecute(context => _message.TrySetResult(context.Message)));
                cfg.SagaMessage<Start>(m => m.UseExecute(context => _consumerMessage.TrySetResult(Tuple.Create(context.Saga, context.Message))));
            });
        }


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
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; private set; }
        }
    }
}
