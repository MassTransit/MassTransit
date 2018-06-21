namespace MassTransit.Containers.Tests
{
    using Lamar;
    using LamarIntegration;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    public class Lamar_Consumer :
        When_registering_a_consumer
    {
        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

        public Lamar_Consumer()
        {
            _container = new Container(x =>
            {
                x.For<SimpleConsumer>().Use<SimpleConsumer>();
                x.For<ISimpleConsumerDependency>().Use<SimpleConsumerDependency>();
                x.For<AnotherMessageConsumer>().Use<AnotherMessageConsumerImpl>();
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    public class Lamar_Saga :
        When_registering_a_saga
    {
        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

        public Lamar_Saga()
        {
            _container = new Container(x =>
            {
                x.For(typeof(ISagaRepository<>))
                    .Use(typeof(InMemorySagaRepository<>))
                    .Singleton();

                x.ForConcreteType<SimpleSaga>();
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
