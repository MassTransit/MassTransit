namespace MassTransit.Containers.Tests.Common_Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Sagas;


    public abstract class Common_SagaStateMachine :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_the_first_event()
        {
            Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();
            Task<ConsumeContext<TestUpdated>> updated = ConnectPublishHandler<TestUpdated>();

            await InputQueueSendEndpoint.Send(new StartTest
            {
                CorrelationId = NewId.NextGuid(),
                TestKey = "Unique"
            });

            await started;

            await InputQueueSendEndpoint.Send(new UpdateTest {TestKey = "Unique"});

            await updated;
        }

        protected void ConfigureRegistration<T>(IRegistrationConfigurator<T> configurator)
            where T : class
        {
            configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                .InMemoryRepository();

            configurator.AddBus(provider => BusControl);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(Registration);
        }

        protected abstract IRegistration Registration { get; }
    }
}
