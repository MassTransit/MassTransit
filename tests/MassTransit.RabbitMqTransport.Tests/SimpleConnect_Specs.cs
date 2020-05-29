namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Using_a_simple_connection_to_rabbit :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_be_good()
        {
        }

        [Test]
        public async Task Should_receive_a_message()
        {
            Task<ConsumeContext<PongMessage>> response = SubscribeHandler<PongMessage>();

            await InputQueueSendEndpoint.Send(new PingMessage(), x =>
            {
                x.ResponseAddress = Bus.Address;
            });

            await response;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Using_a_simple_connection_to_rabbit_to_publish :
        RabbitMqTestFixture
    {
        [Test]
        public void Should_be_good()
        {
        }

        [Test]
        public async Task Should_receive_a_message()
        {
            Task<ConsumeContext<PongMessage>> response = SubscribeHandler<PongMessage>();

            await Bus.Publish(new PingMessage(), x =>
            {
                x.ResponseAddress = Bus.Address;
            });

            await response;
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            Handler<PingMessage>(configurator, context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }
    }


    [TestFixture]
    public class Publishing_without_a_listener :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_receive_a_message()
        {
            await Bus.Publish(new PingMessage(), x =>
            {
                x.ResponseAddress = Bus.Address;
            });
        }
    }
}
