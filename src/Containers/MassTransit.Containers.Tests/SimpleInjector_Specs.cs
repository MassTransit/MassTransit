namespace MassTransit.Containers.Tests
{
    using MassTransit.Containers.Tests.Scenarios;
    using MassTransit.Saga;
    using MassTransit.SimpleInjectorIntegration;

    using NUnit.Framework;

    using SimpleInjector;
    using SimpleInjector.Extensions.ExecutionContextScoping;
    
    public class SimpleInjector_Consumer :
            When_registering_a_consumer
    {
        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly Container _container;

        public SimpleInjector_Consumer()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            _container.Register<SimpleConsumer>(Lifestyle.Scoped);
            _container.Register<ISimpleConsumerDependency, SimpleConsumerDependency>(
                Lifestyle.Scoped);
            _container.Register<AnotherMessageConsumer, AnotherMessageConsumerImpl>(
                Lifestyle.Scoped);
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    public class SimpleInjector_Saga :
        When_registering_a_saga
    {
        [TestFixtureTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly Container _container;

        public SimpleInjector_Saga()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            _container.Register(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>),
                Lifestyle.Singleton);
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.Saga<SimpleSaga>(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
