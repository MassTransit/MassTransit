namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using Saga;
    using Scenarios.StateMachines;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjector_SagaStateMachine :
        Common_SagaStateMachine
    {
        readonly Container _container;

        public SimpleInjector_SagaStateMachine()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddSagaStateMachine<TestStateMachineSaga, TestInstance>();
                cfg.AddBus(() => BusControl);
            });

            _container.Register<PublishTestStartedActivity>();

            _container.Register(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>),
                Lifestyle.Singleton);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureSagaStateMachine(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<TestInstance>(_container);
        }
    }
}
