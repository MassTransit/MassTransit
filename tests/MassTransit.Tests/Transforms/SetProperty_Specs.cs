namespace MassTransit.Tests.Transforms
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Setting_a_property_on_the_original_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            await InputQueueSendEndpoint.Send(new A { First = "Hello" });

            ConsumeContext<A> result = await _received;

            Assert.Multiple(() =>
            {
                Assert.That(result.Message.First, Is.EqualTo("Hello"));
                Assert.That(result.Message.Second, Is.EqualTo("World"));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _received;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseTransform<A>(t =>
            {
                t.Replace = true;
                // Replace modifies the original message, versus Set which leaves the original message unmodified
                t.Set(x => x.Second, context => "World");
            });

            _received = Handled<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }


    [TestFixture]
    public class Setting_a_property_on_a_new_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            await InputQueueSendEndpoint.Send(new A { First = "Hello" });

            ConsumeContext<A> result = await _received;

            Assert.Multiple(() =>
            {
                Assert.That(result.Message.First, Is.EqualTo("Hello"));
                Assert.That(result.Message.Second, Is.EqualTo("World"));
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<A>> _received;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseTransform<A>(t =>
            {
                t.Set(x => x.Second, context => "World");
            });

            _received = Handled<A>(configurator);
        }


        class A
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }


    [TestFixture]
    public class Setting_a_property_on_the_original_message_interface :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            Task<ConsumeContext<IA>> unmodified = await ConnectPublishHandler<IA>();

            await Bus.Publish(new A { First = "Hello" });

            ConsumeContext<IA> result = await _received;
            ConsumeContext<IA> original = await unmodified;
            var tweaked = await _tweaked.Task;

            Assert.Multiple(() =>
            {
                Assert.That(result.Message.First, Is.EqualTo("Hello"));
                Assert.That(result.Message.Second, Is.EqualTo("World"));
                Assert.That(tweaked.Second, Is.EqualTo("World"));

                Assert.That(original.Message.First, Is.EqualTo("Hello"));
                Assert.That(original.Message.Second, Is.Null);
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<IA>> _received;
        #pragma warning restore NUnit1032
        TaskCompletionSource<IA> _tweaked;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _received = Handled<IA>(configurator);

            _tweaked = GetTask<IA>();

            configurator.Handler<IA>(async context => _tweaked.TrySetResult(context.Message), x => x.UseTransform(t =>
            {
                t.Replace = true;
                t.Set(p => p.Second, _ => "World");
            }));
        }


        public interface IA
        {
            string First { get; }
            string Second { get; }
        }


        class A : IA
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }


    [TestFixture]
    public class Setting_a_property_on_the_message_interface :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_the_message_property()
        {
            Task<ConsumeContext<IA>> unmodified = await ConnectPublishHandler<IA>();

            await Bus.Publish(new A { First = "Hello" });

            ConsumeContext<IA> result = await _received;
            ConsumeContext<IA> original = await unmodified;
            var tweaked = await _tweaked.Task;

            Assert.Multiple(() =>
            {
                Assert.That(result.Message.First, Is.EqualTo("Hello"));
                Assert.That(result.Message.Second, Is.Null);

                Assert.That(tweaked.Second, Is.EqualTo("World"));

                Assert.That(original.Message.First, Is.EqualTo("Hello"));
                Assert.That(original.Message.Second, Is.Null);
            });
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<IA>> _received;
        #pragma warning restore NUnit1032
        TaskCompletionSource<IA> _tweaked;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _received = Handled<IA>(configurator);

            _tweaked = GetTask<IA>();

            configurator.Handler<IA>(async context => _tweaked.TrySetResult(context.Message), x =>
            {
                x.UseTransform(t =>
                {
                    t.Set(p => p.Second, context => "World");
                });
            });
        }


        public interface IA
        {
            string First { get; }
            string Second { get; }
        }


        class A : IA
        {
            public string First { get; set; }
            public string Second { get; set; }
        }
    }
}
