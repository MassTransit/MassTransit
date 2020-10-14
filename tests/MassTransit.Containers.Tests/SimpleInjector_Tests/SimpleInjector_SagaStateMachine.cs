namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using NUnit.Framework;
    using SimpleInjector;
    using TestFramework.Sagas;


    [TestFixture]
    public class SimpleInjector_SagaStateMachine :
        Common_SagaStateMachine
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjector_SagaStateMachine()
        {
            _container = new Container();
            _container.SetRequiredOptions();

            _container.AddMassTransit(ConfigureRegistration);

            _container.Register<PublishTestStartedActivity>();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }
}
