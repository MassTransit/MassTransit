namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_message_in_a_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_source_address_from_the_endpoint()
        {
            Task<ConsumeContext<PongMessage>> responseHandled = await ConnectPublishHandler<PongMessage>();

            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _handled;

            ConsumeContext<PongMessage> responseContext = await responseHandled;

            Assert.That(responseContext.SourceAddress, Is.EqualTo(InputQueueAddress));
        }

        #pragma warning disable NUnit1032
        Task<ConsumeContext<PingMessage>> _handled;
        #pragma warning restore NUnit1032

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handled = Handler<PingMessage>(configurator, context => context.Publish(new PongMessage(context.Message.CorrelationId)));
        }
    }
}
