namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Sending_a_request_to_another_scope :
        TwoScopeAzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            var pingMessage = new PingMessage();

            Response<PongMessage> response = await _requestClient.GetResponse<PongMessage>(pingMessage);

            Assert.That(response.Message.CorrelationId, Is.EqualTo(pingMessage.CorrelationId));
        }

        Task<ConsumeContext<PingMessage>> _handler;
        IRequestClient<PingMessage> _requestClient;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handler<PingMessage>(configurator, context => context.RespondAsync(new PongMessage(context.Message.CorrelationId)));
        }

        protected override void ConfigureSecondInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = SecondBus.CreateRequestClient<PingMessage>(InputQueueAddress, TestTimeout);
        }
    }
}
