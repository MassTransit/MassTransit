namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
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
            var clientFactory = Bus.CreateReplyToClientFactory();

            IRequestClient<PingMessage> client = clientFactory.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);

            Response<PongMessage> response = await client.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<PingMessage>(x => x.RespondAsync<PongMessage>(x.Message));
        }
    }


    [TestFixture]
    public class Forwarding_a_message_using_reply_to :
        RabbitMqTestFixture
    {
        Uri _serverAddress;

        [Test]
        public async Task Should_deliver_the_response()
        {
            var clientFactory = Bus.CreateReplyToClientFactory();

            IRequestClient<PingMessage> client = clientFactory.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);

            Response<PongMessage> response = await client.GetResponse<PongMessage>(new PingMessage());
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("forward-reply-to", x =>
            {
                x.ConfigureConsumeTopology = false;

                x.Handler<PingMessage>(context => context.RespondAsync<PongMessage>(context.Message));

                _serverAddress = x.InputAddress;
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<PingMessage>(x => x.Forward(_serverAddress));
        }
    }
}
