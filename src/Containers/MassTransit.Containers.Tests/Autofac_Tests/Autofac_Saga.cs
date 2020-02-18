namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    public class Autofac_Saga :
        Common_Saga
    {
        readonly IContainer _container;

        public Autofac_Saga()
        {
            _container = new ContainerBuilder()
                .AddMassTransit(ConfigureRegistration)
                .Build();
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
            return _container.Resolve<ISagaRepository<T>>();
        }
    }


    public class Autofac_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IContainer _container;

        public Autofac_Saga_Endpoint()
        {
            _container = new ContainerBuilder()
                .AddMassTransit(ConfigureRegistration)
                .Build();
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
            return _container.Resolve<ISagaRepository<T>>();
        }
    }
}
