namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using Common_Tests;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
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

            _container.AddMassTransit(cfg =>
            {
                cfg.AddSaga<SimpleSaga>();
                cfg.AddBus(() => BusControl);
            });

            _container.Register(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>), Lifestyle.Singleton);
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
    public class SimpleInjector_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly Container _container;

        public SimpleInjector_Saga_Endpoint()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(x =>
            {
                x.AddSaga<SimpleSaga>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(() => BusControl);
            });

            _container.RegisterSingleton<ISagaRepository<SimpleSaga>, InMemorySagaRepository<SimpleSaga>>();
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
