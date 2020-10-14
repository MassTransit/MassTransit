namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using SimpleInjector;
    using SimpleInjector_Tests;


    public class SimpleInjector_Consumer :
        When_registering_a_consumer
    {
        [TearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        readonly Container _container;

        public SimpleInjector_Consumer()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(cfg => cfg.AddConsumer<SimpleConsumer>());

            _container.Register<ISimpleConsumerDependency, SimpleConsumerDependency>(
                Lifestyle.Scoped);
            _container.Register<AnotherMessageConsumer, AnotherMessageConsumerImpl>(
                Lifestyle.Scoped);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(_container.GetInstance<IBusRegistrationContext>());
        }
    }


    public class SimpleInjector_Saga :
        When_registering_a_saga
    {
        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        readonly Container _container;

        public SimpleInjector_Saga()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();
            _container.RegisterInMemorySagaRepository<SimpleSaga>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga<SimpleSaga>(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
