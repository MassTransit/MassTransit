namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
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

            _container.AddMassTransit(ConfigureRegistration);

            _container.Register<PublishTestStartedActivity>();
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
