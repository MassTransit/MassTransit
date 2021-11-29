namespace MassTransit.Tests.Conventional
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Configuring_a_consumer_by_custom_convention :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_find_the_message_handlers()
        {
            await Bus.Publish<MessageA>(new { Value = "Hello" });
            await Bus.Publish<MessageB>(new { Name = "World" });

            await _receivedA.Task;
            await _receivedB.Task;
        }

        [TearDown]
        public Task TearDown()
        {
            ConsumerConvention.Remove<CustomConsumerConvention>();
            return Task.CompletedTask;
        }

        TaskCompletionSource<MessageA> _receivedA;
        TaskCompletionSource<MessageB> _receivedB;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _receivedA = GetTask<MessageA>();
            _receivedB = GetTask<MessageB>();

            configurator.UseMessageRetry(r => r.Interval(1, 100));

            ConsumerConvention.Register<CustomConsumerConvention>();

            configurator.Consumer(typeof(CustomHandler), type => new CustomHandler(_receivedA, _receivedB));
        }


        class CustomHandler :
            IHandler<MessageA>,
            IHandler<MessageB>
        {
            readonly TaskCompletionSource<MessageA> _receivedA;
            readonly TaskCompletionSource<MessageB> _receivedB;

            public CustomHandler(TaskCompletionSource<MessageA> receivedA, TaskCompletionSource<MessageB> receivedB)
            {
                _receivedA = receivedA;
                _receivedB = receivedB;
            }

            public void Handle(MessageA message)
            {
                _receivedA.TrySetResult(message);
            }

            public void Handle(MessageB message)
            {
                _receivedB.TrySetResult(message);
            }
        }


        public interface MessageA
        {
            string Value { get; }
        }


        public interface MessageB
        {
            string Name { get; }
        }
    }


    [TestFixture]
    public class Configuring_a_consumer_by_default_conventions :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_find_the_message_handlers()
        {
            await Bus.Publish<MessageA>(new { Value = "Hello" });

            await _receivedA.Task;
        }

        TaskCompletionSource<MessageA> _receivedA;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _receivedA = GetTask<MessageA>();

            configurator.Consumer(typeof(DefaultConventionHandlers), type => new DefaultConventionHandlers(_receivedA));
        }


        class DefaultConventionHandlers :
            IConsumer<MessageA>
        {
            readonly TaskCompletionSource<MessageA> _receivedA;

            public DefaultConventionHandlers(TaskCompletionSource<MessageA> receivedA)
            {
                _receivedA = receivedA;
            }

            public Task Consume(ConsumeContext<MessageA> context)
            {
                _receivedA.TrySetResult(context.Message);
                return Task.FromResult(0);
            }
        }


        public interface MessageA
        {
            string Value { get; }
        }
    }
}
