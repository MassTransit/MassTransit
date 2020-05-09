namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using TestFramework.Sagas;


    [TestFixture]
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

        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();
    }
}
