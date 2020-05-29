namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_message_on_the_bus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_converation_id()
        {
            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> context = await _handled;

            context.ConversationId.HasValue.ShouldBe(true);
        }

        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Sending_a_message :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_converation_id()
        {
            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _handled;

            context.ConversationId.HasValue.ShouldBe(true);
        }

        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_from_a_handler :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_include_a_converation_id()
        {
            Task<ConsumeContext<PongMessage>> responseHandled = ConnectPublishHandler<PongMessage>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _handled;

            context.ConversationId.HasValue.ShouldBe(true);

            ConsumeContext<PongMessage> responseContext = await responseHandled;

            responseContext.ConversationId.HasValue.ShouldBe(true);

            responseContext.ConversationId.ShouldBe(context.ConversationId);
        }

        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handler<PingMessage>(configurator, context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }
    }
}
