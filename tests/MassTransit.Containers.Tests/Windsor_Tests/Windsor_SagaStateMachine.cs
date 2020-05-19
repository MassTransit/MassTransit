namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using TestFramework.Sagas;


    [TestFixture]
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

        protected override MassTransit.IRegistration Registration => _container.Resolve<MassTransit.IRegistration>();
    }
}
