namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using MassTransit.Saga;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_message_correlation_is_not_configured :
        InMemoryTestFixture
    {
        [Test]
        public void Should_retry_the_status_message()
        {
            TestDelegate invocation = () => Bus.ConnectStateMachineSaga(_machine, _repository);

            Assert.Throws<ConfigurationException>(invocation);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
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

                Initially(
                    When(Firsted)
                        .Then(context => Console.WriteLine("Started: {0}", context.Instance.CorrelationId))
                        .TransitionTo(Running));

                During(Running,
                    When(Seconded)
                        .Then(context => Console.WriteLine("Status check!")));
            }

            public State Running { get; private set; }

            public Event<First> Firsted { get; private set; }
            public Event<Second> Seconded { get; private set; }
        }


        class Second
        {
            public string ServiceName { get; set; }
        }


        class First :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
