namespace MassTransit.Containers.Tests
{
    using System;
    using System.Threading.Tasks;
    using Lamar;
    using NUnit.Framework;
    using Scenarios;
    using Shouldly;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class Lamar_Idiomatic :
        AsyncTestFixture
    {
        [Test]
        public async Task Should_work_with_the_registry()
        {
            var bus = _container.GetInstance<IBusControl>();

            BusHandle busHandle = await bus.StartAsync();

            await busHandle.Ready;

            ISendEndpoint endpoint = await bus.GetSendEndpoint(new Uri("loopback://localhost/input_queue"));

            const string name = "Joe";

            await endpoint.Send(new SimpleMessageClass(name));

            var lastConsumer = await SimpleConsumer.LastConsumer;
            lastConsumer.ShouldNotBeNull();

            var last = await lastConsumer.Last;
            last.Name
                .ShouldBe(name);

            var wasDisposed = await lastConsumer.Dependency.WasDisposed;
            wasDisposed
                .ShouldBeTrue(); // Dependency was not disposed");

            lastConsumer.Dependency.SomethingDone
                .ShouldBeTrue(); // Dependency was disposed before consumer executed");
        }

        public Lamar_Idiomatic()
            : base(new InMemoryTestHarness())
        { }

        Container _container;

        [OneTimeSetUp]
        public void Setup()
        {
            _container = new Container(x =>
            {
                x.IncludeRegistry<BusServiceRegistry>();
                x.IncludeRegistry<ConsumerServiceRegistry>();
            });
        }


        class ConsumerServiceRegistry : ServiceRegistry
        {
            public ConsumerServiceRegistry()
            {
                this.AddMassTransit(cfg => cfg.AddConsumer<SimpleConsumer>());

                For<ISimpleConsumerDependency>().Use<SimpleConsumerDependency>();
                For<AnotherMessageConsumer>().Use<AnotherMessageConsumerImpl>();
            }
        }


        class BusServiceRegistry : ServiceRegistry
        {
            public BusServiceRegistry()
            {
                For<IBusControl>().Use(context => Bus.Factory.CreateUsingInMemory(x => x.ReceiveEndpoint("input_queue", e => e.ConfigureConsumers(context))))
                .Singleton();
            }
        }


        [OneTimeTearDown]
        public void TearDown()
        {
            _container.Dispose();
        }
    }
}
