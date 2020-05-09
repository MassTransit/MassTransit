namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using Saga;


    [TestFixture]
    public class Windsor_Saga :
        Common_Saga
    {
        readonly IWindsorContainer _container;

        public Windsor_Saga()
        {
            _container = new WindsorContainer()
                .AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override MassTransit.IRegistration Registration => _container.Resolve<IRegistration>();

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class Windsor_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IWindsorContainer _container;

        public Windsor_Saga_Endpoint()
        {
            _container = new WindsorContainer()
                .AddMassTransit(ConfigureRegistration);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.Resolve<IRegistration>();

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }
}
