namespace MassTransit.Tests.Pipeline
{
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class ContentFilter_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_only_get_one_message()
        {
            await InputQueueSendEndpoint.Send<TestMessage>(new { Key = "DENY" });
            await InputQueueSendEndpoint.Send<TestMessage>(new { Key = "ACCEPT" });

            await _accepted;
            await _denied;

            await Task.Delay(50);

            Assert.That(_consumer.Count, Is.EqualTo(1));
        }

        MyConsumer _consumer;
        #pragma warning disable NUnit1032
        Task<ConsumeContext<TestMessage>> _accepted;
        Task<ConsumeContext<TestMessage>> _denied;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _consumer = new MyConsumer();

            configurator.Consumer(() => _consumer, x =>
            {
                x.Message<TestMessage>(v => v.UseContextFilter(async context => context.Message.Key == "ACCEPT"));
            });

            _accepted = Handled<TestMessage>(configurator, x => x.Message.Key == "ACCEPT");
            _denied = Handled<TestMessage>(configurator, x => x.Message.Key == "DENY");
        }


        class MyConsumer :
            IConsumer<TestMessage>
        {
            int _count;

            public int Count => _count;

            public Task Consume(ConsumeContext<TestMessage> context)
            {
                Interlocked.Increment(ref _count);

                return Task.CompletedTask;
            }
        }


        public interface TestMessage
        {
            string Key { get; }
        }
    }
}
