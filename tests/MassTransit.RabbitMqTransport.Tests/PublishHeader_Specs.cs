namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_message_in_a_consumer_with_baggage :
        RabbitMqTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_source_address_from_the_endpoint()
        {
            Task<ConsumeContext<PongMessage>> responseHandled = await ConnectPublishHandler<PongMessage>(pongContext =>
            {
                return Activity.Current?.Baggage.Any(x => x.Key.Equals("Suitcase")) ?? false;
            });

            await InputQueueSendEndpoint.Send(new PingMessage());

            ConsumeContext<PingMessage> context = await _handled;

            ConsumeContext<PongMessage> responseContext = await responseHandled;

            responseContext.SourceAddress.ShouldBe(InputQueueAddress);
        }

        Task<ConsumeContext<PingMessage>> _handled;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handled = Handler<PingMessage>(configurator, context =>
            {
                Activity.Current?.AddBaggage("Suitcase", "Full of cash");

                return context.Publish(new PongMessage(context.Message.CorrelationId));
            });
        }
    }
}
