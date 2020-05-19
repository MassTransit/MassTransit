namespace MassTransit.Tests.Transforms
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    [TestFixture]
    public class Transforming_a_message_when_sent_but_published :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_affect_published_messages()
        {
            await Bus.Publish(new A {First = "Hello"});

            ConsumeContext<A> result = await _received;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe(null);
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(s => s.UseTransform<A>(t =>
            {
                t.Replace = true;
                t.Set(x => x.Second, context => "World");
            }));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _received = Handled<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }


    [TestFixture]
    public class Transforming_a_message_when_sent :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_change_the_property()
        {
            await InputQueueSendEndpoint.Send(new A {First = "Hello"});

            ConsumeContext<A> result = await _received;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe("World");
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureSend(s => s.UseTransform<A>(t =>
            {
                t.Replace = true;
                t.Set(x => x.Second, context => "World");
            }));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _received = Handled<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }


    [TestFixture]
    public class Transforming_a_message_when_published :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_transform_on_publish()
        {
            await Bus.Publish(new A {First = "Hello"});

            ConsumeContext<A> result = await _received;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe("World");
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigurePublish(s => s.UseTransform<A>(t =>
            {
                t.Replace = true;
                t.Set(x => x.Second, context => "World");
            }));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _received = Handled<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }


    [TestFixture]
    public class Transforming_a_message_when_published_but_sent :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_transform_on_send()
        {
            await InputQueueSendEndpoint.Send(new A {First = "Hello"});

            ConsumeContext<A> result = await _received;

            result.Message.First.ShouldBe("Hello");
            result.Message.Second.ShouldBe(null);
        }

        Task<ConsumeContext<A>> _received;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigurePublish(s => s.UseTransform<A>(t =>
            {
                t.Replace = true;

                t.Set(x => x.Second, context => "World");
            }));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _received = Handled<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }
}
