namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_the_reply_to_address :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_deliver_the_response()
        {
            var clientFactory = await Bus.CreateReplyToClientFactory();

            var client = clientFactory.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);

            Response<PongMessage> response = await client.GetResponse<PongMessage>(new PingMessage());
        }

        [OneTimeSetUp]
        public void Setup()
        {
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<PingMessage>(x =>
            {
                return x.RespondAsync<PongMessage>(x.Message);
            });
        }
    }
}
