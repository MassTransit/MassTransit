namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using Saga;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_Saga :
        Common_Saga
    {
        readonly Container _container;

        public SimpleInjector_Saga()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(ConfigureRegistration);
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

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class SimpleInjector_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly Container _container;

        public SimpleInjector_Saga_Endpoint()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(ConfigureRegistration);
        }

        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        protected override IRegistration Registration => _container.GetInstance<IRegistration>();

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
