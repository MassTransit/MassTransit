namespace MassTransit.Tests.AutomatonymousIntegration
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Publishing_a_message_from_a_saga_state_machine :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_published_message()
        {
            Task<ConsumeContext<StartupComplete>> messageReceived = ConnectPublishHandler<StartupComplete>();

            var message = new Start();

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<StartupComplete> received = await messageReceived;

            Assert.AreEqual(message.CorrelationId, received.Message.TransactionId);

            Assert.IsTrue(received.InitiatorId.HasValue, "The initiator should be copied from the CorrelationId");

            Assert.AreEqual(received.InitiatorId.Value, message.CorrelationId, "The initiator should be the saga CorrelationId");

            Assert.AreEqual(received.SourceAddress, InputQueueAddress, "The published message should have the input queue source address");

            Guid? saga =
                await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId && Equals(x.CurrentState, _machine.Running), TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        [Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            var result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
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
                        .Publish(context => new StartupComplete {TransactionId = context.Data.CorrelationId})
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


        class StartupComplete
        {
            public Guid TransactionId { get; set; }
        }
    }
}
