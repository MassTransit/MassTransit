namespace MassTransit.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_message_to_a_consumer :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received()
        {
            await _requestClient.Request(new PingMessage());
        }

        IRequestClient<PingMessage, PongMessage> _requestClient;

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = CreateRequestClient<PingMessage, PongMessage>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer<Consumer>();
        }


        class Consumer :
            IConsumer<PingMessage>
        {
            public async Task Consume(ConsumeContext<PingMessage> context)
            {
                await context.RespondAsync(new PongMessage(context.Message.CorrelationId));
            }
        }
    }
}
