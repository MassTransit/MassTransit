namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios;


    [TestFixture]
    public class Windsor_Consumer :
        Common_Consumer
    {
        readonly IWindsorContainer _container;

        public Windsor_Consumer()
        {
            var container = new WindsorContainer();
            container.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleConsumer>();
                x.AddBus(provider => BusControl);
            });

            container.Register(Component.For<ISimpleConsumerDependency>().ImplementedBy<SimpleConsumerDependency>().LifestyleScoped(),
                Component.For<AnotherMessageConsumer>().ImplementedBy<AnotherMessageConsumerImpl>().LifestyleScoped()
            );

            _container = container;
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Windsor_Consumer_From_Container :
        Common_Consumer
    {
        readonly IWindsorContainer _container;

        public Windsor_Consumer_From_Container()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<SimpleConsumer>()
                    .LifestyleScoped());

            container.AddMassTransit(x =>
            {
                x.AddConsumersFromContainer(container);

                x.AddBus(provider => BusControl);
            });

            container.Register(Component.For<ISimpleConsumerDependency>().ImplementedBy<SimpleConsumerDependency>().LifestyleScoped(),
                Component.For<AnotherMessageConsumer>().ImplementedBy<AnotherMessageConsumerImpl>().LifestyleScoped()
            );

            _container = container;
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }


    [TestFixture]
    public class Windsor_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly IWindsorContainer _container;

        public Windsor_Consumer_Endpoint()
        {
            var container = new WindsorContainer();
            container.AddMassTransit(x =>
            {
                x.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(provider => BusControl);
            });

            container.Register(Component.For<ISimpleConsumerDependency>().ImplementedBy<SimpleConsumerDependency>().LifestyleScoped(),
                Component.For<AnotherMessageConsumer>().ImplementedBy<AnotherMessageConsumerImpl>().LifestyleScoped()
            );

            _container = container;
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IBusRegistrationContext Registration => _container.Resolve<IBusRegistrationContext>();
    }
}
