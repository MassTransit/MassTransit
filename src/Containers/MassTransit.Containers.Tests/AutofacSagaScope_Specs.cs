namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Sagas;


    [TestFixture]
    public class Autofac_SagaScope :
        InMemoryTestFixture
    {
        readonly IContainer _container;
        bool _isCalled;

        public Autofac_SagaScope()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                    .InMemoryRepository();

                x.ConfigureScope = (containerBuilder, context) => _isCalled = true;

                x.AddBus(provider => BusControl);
            });
            builder.RegisterType<PublishTestStartedActivity>();

            _container = builder.Build();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(_container);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        [Test]
        public async Task Should_configure_scope()
        {
            Task<ConsumeContext<TestStarted>> started = ConnectPublishHandler<TestStarted>();

            await InputQueueSendEndpoint.Send(new StartTest
            {
                CorrelationId = NewId.NextGuid(),
                TestKey = "Unique"
            });

            await started;

            Assert.IsTrue(_isCalled);
        }
    }
}
