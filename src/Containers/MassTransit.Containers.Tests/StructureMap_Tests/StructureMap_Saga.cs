namespace MassTransit.Containers.Tests.StructureMap_Tests
{
    using Common_Tests;
    using StructureMap;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    [TestFixture]
    public class StructureMap_Saga :
        Common_Saga
    {
        readonly IContainer _container;

        public StructureMap_Saga()
        {
            _container = new Container(expression => expression.AddMassTransit(ConfigureRegistration));
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
    public class StructureMap_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IContainer _container;

        public StructureMap_Saga_Endpoint()
        {
            _container = new Container(expression => expression.AddMassTransit(ConfigureRegistration));
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
