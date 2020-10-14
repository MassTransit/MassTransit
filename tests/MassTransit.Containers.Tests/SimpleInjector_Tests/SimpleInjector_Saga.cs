namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using NUnit.Framework;
    using Saga;
    using SimpleInjector;


    [TestFixture]
    public class SimpleInjector_Saga :
        Common_Saga
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjector_Saga()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class SimpleInjector_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        readonly Container _container;

        public SimpleInjector_Saga_Endpoint()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(ConfigureRegistration);
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
