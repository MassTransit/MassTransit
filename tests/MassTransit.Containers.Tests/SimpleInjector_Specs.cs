namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using SimpleInjector;
    using SimpleInjector_Tests;
    using TestFramework;
    using TestFramework.Messages;


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
        readonly Container _container;

        public SimpleInjector_Saga()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();
            _container.RegisterInMemorySagaRepository<SimpleSaga>();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
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


    public class When_creating_a_consumer_scope :
        InMemoryTestFixture
    {
        readonly Container _container;

        public When_creating_a_consumer_scope()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(cfg => cfg.AddConsumer<ScopeConsumer>());
        }

        [Test]
        public async Task Should_have_the_scope_in_consume()
        {
            var client = Bus.CreateRequestClient<PingMessage>();

            var response = await client.GetResponse<PongMessage>(new PingMessage());
        }

        [TearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumers(_container.GetInstance<IBusRegistrationContext>());
        }


        class ScopeConsumer :
            IConsumer<PingMessage>
        {
            readonly Container _container;

            public ScopeConsumer(Container container)
            {
                _container = container;
            }

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                if (Lifestyle.Scoped.GetCurrentScope(_container) == null)
                    throw new InvalidOperationException("The container scope is null");

                return context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }
    }
}
