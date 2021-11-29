namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Sending_a_message_from_a_state_machine :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_published_message()
        {
            var message = new Start();

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<StartupComplete> received = await _handled;

            Assert.AreEqual(message.CorrelationId, received.Message.TransactionId);

            Assert.IsTrue(received.InitiatorId.HasValue, "The initiator should be copied from the CorrelationId");

            Assert.AreEqual(message.CorrelationId, received.InitiatorId.Value, "The initiator should be the saga CorrelationId");

            Assert.AreEqual(InputQueueAddress, received.SourceAddress, "The published message should have the input queue source address");

            Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, _machine.Running, TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.ReceiveEndpoint("observer", e =>
            {
                _handlerAddress = e.InputAddress;

                _handled = Handled<StartupComplete>(e);
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine(_handlerAddress);
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;

        Task<ConsumeContext<StartupComplete>> _handled;
        Uri _handlerAddress;


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine(Uri sendAddress)
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Started)
                        .Send(sendAddress, context => new StartupComplete { TransactionId = context.Data.CorrelationId })
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

            public Guid CorrelationId { get; set; }
        }


        class StartupComplete
        {
            public Guid TransactionId { get; set; }
        }
    }
}
