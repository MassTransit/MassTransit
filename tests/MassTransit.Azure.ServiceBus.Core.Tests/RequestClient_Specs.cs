namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
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


    [TestFixture]
    public class Sending_a_request_using_the_request_client_with_raw_json :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_receive_the_response()
        {
            using RequestHandle<PingMessage> requestHandle = _requestClient.Create(new PingMessage());

            Response<PongMessage> response = await requestHandle.GetResponse<PongMessage>();

            response.Message.CorrelationId.ShouldBe(_ping.Result.Message.CorrelationId);
        }

        Task<ConsumeContext<PingMessage>> _ping;
        IRequestClient<PingMessage> _requestClient;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            base.ConfigureServiceBusBus(configurator);

            configurator.UseRawJsonSerializer();
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _requestClient = Bus.CreateRequestClient<PingMessage>(TestTimeout);
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _ping = Handler<PingMessage>(configurator, async x => await x.RespondAsync(new PongMessage(x.Message.CorrelationId)));
        }
    }
}
