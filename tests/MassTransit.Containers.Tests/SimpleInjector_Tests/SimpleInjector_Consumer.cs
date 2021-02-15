namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System.Threading.Tasks;
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios;
    using SimpleInjector;


    [TestFixture]
    public class SimpleInjector_Consumer :
        Common_Consumer
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        readonly Container _container;

        public SimpleInjector_Consumer()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SimpleConsumer>();
                cfg.AddBus(() => BusControl);
            });

            _container.Register<ISimpleConsumerDependency, SimpleConsumerDependency>(Lifestyle.Scoped);

            _container.Register<AnotherMessageConsumer, AnotherMessageConsumerImpl>(Lifestyle.Scoped);
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class SimpleInjector_Consumer_ConfigureEndpoint :
        Common_Consumer_ConfigureEndpoint
    {
        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        readonly Container _container;

        public SimpleInjector_Consumer_ConfigureEndpoint()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(ConfigureRegistration);

            _container.Collection.Register<IConfigureReceiveEndpoint>(typeof(DoNotPublishFaults));
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    [TestFixture]
    public class SimpleInjector_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        [TearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }

        readonly Container _container;

        public SimpleInjector_Consumer_Endpoint()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                cfg.AddBus(() => BusControl);
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetInstance<IBusRegistrationContext>();
    }


    public class SimpleInjector_Consumer_Connect :
        Common_Consumer_Connect
    {
        readonly Container _container;

        public SimpleInjector_Consumer_Connect()
        {
            _container = new Container();
            _container.SetMassTransitContainerOptions();

            _container.AddMassTransit(ConfigureRegistration);

            _container.RegisterSingleton(() => MessageCompletion);
        }

        protected override IReceiveEndpointConnector Connector => _container.GetInstance<IReceiveEndpointConnector>();

        [Test]
        public void Should_be_a_valid_container()
        {
            _container.Verify();
        }

        [OneTimeTearDown]
        public async Task Close_container()
        {
            await _container.DisposeAsync();
        }
    }
}
