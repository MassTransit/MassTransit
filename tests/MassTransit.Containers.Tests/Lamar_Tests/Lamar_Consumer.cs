namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;


    [TestFixture]
    public class Lamar_Consumer :
        Common_Consumer
    {
        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

        public Lamar_Consumer()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<SimpleConsumer>();
                    cfg.AddBus(context => BusControl);
                });

                registry.For<ISimpleConsumerDependency>()
                    .Use<SimpleConsumerDependency>();

                registry.For<AnotherMessageConsumer>()
                    .Use<AnotherMessageConsumerImpl>();
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Lamar_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

        public Lamar_Consumer_Endpoint()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<SimplerConsumer>()
                        .Endpoint(e => e.Name = "custom-endpoint-name");

                    cfg.AddBus(context => BusControl);
                });
            });
        }

        protected override IBusRegistrationContext Registration => _container.GetRequiredService<IBusRegistrationContext>();
    }
}
