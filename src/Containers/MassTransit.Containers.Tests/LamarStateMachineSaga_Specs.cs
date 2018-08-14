namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Automatonymous;
    using Lamar;
    using AutomatonymousLamarIntegration;
    using LamarIntegration;
    using NUnit.Framework;
    using Saga;
    using Scenarios.StateMachines;
    using TestFramework;


    public class LamarStateMachineSaga_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_first_event_successfully()
        {
            Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();
            Task<ConsumeContext<TestUpdated>> updated = ConnectPublishHandler<TestUpdated>();

            await InputQueueSendEndpoint.Send(new StartTest {CorrelationId = NewId.NextGuid(), TestKey = "Unique"});

            await started;

            await InputQueueSendEndpoint.Send(new UpdateTest {TestKey = "Unique"});

            await updated;
        }

        IContainer _container;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _container = new Container(x =>
            {
                x.For(typeof(ISagaRepository<>)).Use(typeof(InMemorySagaRepository<>)).Singleton();

                x.AddMassTransit();
                
                x.ForConcreteType<PublishTestStartedActivity>();

                x.For<TestStateMachineSaga>().Use<TestStateMachineSaga>().Singleton();
                x.For(typeof(SagaStateMachine<TestInstance>)).Use(typeof(TestStateMachineSaga)).Singleton();
            });

            configurator.LoadStateMachineSagas(_container);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }
    }
}
