namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_a_state_machine_instance_is_created :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_published_message()
        {
            Task<ConsumeContext<StartupComplete>> messageReceived = await ConnectPublishHandler<StartupComplete>();

            var message = new Start("Joe");

            await InputQueueSendEndpoint.Send(message);

            ConsumeContext<StartupComplete> received = await messageReceived;

            Assert.That(received.Message.TransactionId, Is.EqualTo(message.CorrelationId));

            Assert.IsTrue(received.InitiatorId.HasValue, "The initiator should be copied from the CorrelationId");

            Assert.AreEqual(received.InitiatorId.Value, message.CorrelationId, "The initiator should be the saga CorrelationId");

            Assert.AreEqual(received.SourceAddress, InputQueueAddress, "The published message should have the input queue source address");

            Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, x => x.Running, TestTimeout);

            Assert.IsTrue(saga.HasValue);
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
            public DateTime CreateTimestamp { get; set; }
            public string Name { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started, x =>
                {
                    x.SetSagaFactory(context =>
                    {
                        TestContext.Out.WriteLine("CorrelationID: {0} ", context.CorrelationId);
                        TestContext.Out.WriteLine(Encoding.UTF8.GetString(context.ReceiveContext.Body.GetBytes()));
                        return new Instance
                        {
                            // the CorrelationId header is the value that should be used
                            CorrelationId = context.CorrelationId.Value,
                            CreateTimestamp = context.Message.Timestamp,
                            Name = context.Message.Name
                        };
                    });
                });

                Initially(
                    When(Started)
                        .Publish(context => new StartupComplete { TransactionId = context.Message.CorrelationId })
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
            }

            public Start(string name)
            {
                CorrelationId = NewId.NextGuid();
                Name = name;
                Timestamp = DateTime.UtcNow;
            }

            public string Name { get; set; }
            public DateTime Timestamp { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class StartupComplete
        {
            public Guid TransactionId { get; set; }
        }
    }
}
