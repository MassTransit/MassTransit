namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class LocalBusName_Specs :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_get_the_response_to_the_bus()
        {
            IRequestClient<PingMessage> client = Bus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);

            await client.GetResponse<PongMessage>(new PingMessage(), TestCancellationToken);
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.OverrideDefaultBusEndpointQueueName($"super-bus-{NewId.NextGuid():N}");
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            base.ConfigureRabbitMqReceiveEndpoint(configurator);

            configurator.Handler<PingMessage>(context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }
    }
}
