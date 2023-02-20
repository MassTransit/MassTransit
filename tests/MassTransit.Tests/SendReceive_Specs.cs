namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_an_object_to_the_local_bus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_the_interface_of_the_message()
        {
            Task<ConsumeContext<IMessageA>> handler = SubscribeHandler<IMessageA>();

            var message = new MessageA();
            await BusSendEndpoint.Send(message);

            await handler;
        }

        [Test]
        public async Task Should_receive_the_interface_proxy()
        {
            Task<ConsumeContext<IMessageA>> handler = SubscribeHandler<IMessageA>();

            await BusSendEndpoint.Send<IMessageA>(new { });

            await handler;
        }

        [Test]
        public async Task Should_receive_the_proper_message()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>();

            object message = new MessageA();
            await BusSendEndpoint.Send(message);

            await handler;
        }

        [Test]
        public async Task Should_receive_the_proper_message_as_a()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>();

            var message = new MessageA();
            await BusSendEndpoint.Send(message);

            await handler;
        }

        [Test]
        public async Task Should_receive_the_proper_message_as_a_with_request_id()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>(x => x.RequestId.HasValue);

            var requestId = NewId.NextGuid();

            var message = new MessageA();
            await BusSendEndpoint.Send(message, c => c.RequestId = requestId);

            ConsumeContext<MessageA> consumeContext = await handler;

            Assert.That(consumeContext.RequestId, Is.EqualTo(requestId));
        }

        [Test]
        public async Task Should_receive_the_proper_message_type()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>();

            var requestId = NewId.NextGuid();

            object message = new MessageA();
            await BusSendEndpoint.Send(message, typeof(MessageA), c => c.RequestId = requestId);

            ConsumeContext<MessageA> consumeContext = await handler;

            Assert.That(consumeContext.RequestId, Is.EqualTo(requestId));
        }

        [Test]
        public async Task Should_receive_the_proper_message_without_type()
        {
            Task<ConsumeContext<MessageA>> handler = SubscribeHandler<MessageA>();

            var requestId = NewId.NextGuid();

            object message = new MessageA();
            await BusSendEndpoint.Send(message, context => context.RequestId = requestId);

            ConsumeContext<MessageA> consumeContext = await handler;

            Assert.That(consumeContext.RequestId, Is.EqualTo(requestId));
        }
    }
}
