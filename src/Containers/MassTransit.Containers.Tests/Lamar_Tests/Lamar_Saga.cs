namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    [TestFixture]
    public class Lamar_Saga :
        Common_Saga
    {
        readonly IContainer _container;

        public Lamar_Saga()
        {
            _container = new Container(registry => registry.AddMassTransit(ConfigureRegistration));
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureSaga(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<SimpleSaga>(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class Lamar_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IContainer _container;

        public Lamar_Saga_Endpoint()
        {
            _container = new Container(registry => registry.AddMassTransit(ConfigureRegistration));
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
