namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios.StateMachines;


    public class Windsor_SagaStateMachine :
        Common_SagaStateMachine
    {
        readonly IWindsorContainer _container;

        public Windsor_SagaStateMachine()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(ConfigureRegistration);

            _container.Register(Component.For<PublishTestStartedActivity>().LifestyleScoped());
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
